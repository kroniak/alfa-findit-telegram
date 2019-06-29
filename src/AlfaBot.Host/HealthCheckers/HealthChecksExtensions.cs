using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlfaBot.Host.HealthCheckers
{
    /// <summary>
    /// Extensions to adding custom HealthCheckers
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class HealthChecksExtensions
    {
        /// <summary>
        /// Add custom UI and JSON endpoints to pipeline
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns>IApplicationBuilder instance</returns>
        public static void UseCustomHealthCheckEndpoints(this IApplicationBuilder app)
        {
            app.UseHealthChecks(
                    "/health/detail",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        AllowCachingResponses = false,
                        ResultStatusCodes =
                        {
                            [HealthStatus.Healthy] = StatusCodes.Status200OK,
                            [HealthStatus.Degraded] = StatusCodes.Status200OK,
                            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                        },
                        ResponseWriter = WriteResponse
                    })
                .UseHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"),
                    AllowCachingResponses = false
                })
                .UseHealthChecks("/health/live", new HealthCheckOptions
                {
                    // Exclude all checks and return a 200-Ok
                    Predicate = _ => false,
                    AllowCachingResponses = false
                });
        }

        /// <summary>
        /// Add custom Health Checkers for our services
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">Current configuration root</param>
        /// <returns>IHealthChecksBuilder instance</returns>
        public static void AddCustomHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services
                .AddHealthChecks()
                .AddCheck<MemoryHealthCheck>(
                    "memory-check",
                    HealthStatus.Degraded,
                    new[] {"memory"})
                .AddMongoDb(
                    configuration["MONGO"],
                    configuration["DBNAME"] ?? "FindIT",
                    HealthStatus.Unhealthy,
                    new[] {"database", "ready"}
                )
                .AddCheck<QueueCountHealthCheck>(
                    "queue-count-by-hour",
                    HealthStatus.Degraded,
                    new[] {"queue"});
        }

        private static Task WriteResponse(HttpContext httpContext,
            HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }
}