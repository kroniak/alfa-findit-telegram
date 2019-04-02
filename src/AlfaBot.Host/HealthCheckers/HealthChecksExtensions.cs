using System;
using System.Diagnostics.CodeAnalysis;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
                "/health",
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    AllowCachingResponses = false,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
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
                .AddMongoDb(
                    configuration["MONGO"],
                    configuration["DBNAME"] ?? "FindIT",
                    HealthStatus.Unhealthy,
                    new[] {"database"}
                )
                .AddCheck<QueueCountHealthCheck>(
                    "queue-count-by-hour",
                    HealthStatus.Degraded,
                    new[] {"queue"});
        }
    }
}