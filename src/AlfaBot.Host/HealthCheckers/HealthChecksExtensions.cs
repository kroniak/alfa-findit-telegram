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
//            IAlfaBankBot bot
            )
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
//            if (bot == null) throw new ArgumentNullException(nameof(bot));

            services
                .AddHealthChecks()
                .AddMongoDb(
                    configuration["MONGO"],
                    configuration["DBNAME"] ?? "FindIT",
                    HealthStatus.Unhealthy,
                    new[] {"database"}
                )
//                .AddCheck(
//                    "bot-status",
//                    _ =>
//                    {
//                        var status = bot.Ping();
//                        return status ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
//                    },
//                    new[] {"bot"}
//                )
                .AddCheck<QueueCountHealthCheck>(
                    "queue-count-by-hour",
                    HealthStatus.Degraded,
                    new[] {"queue"})
                .AddUrlGroup(
                    option =>
                    {
                        option.AddUri(
                            new Uri("https://api.telegram.org/bot" + configuration["TELEGRAM_TOKEN"]),
                            setup =>
                            {
                                setup.UseGet();
                                setup.ExpectHttpCode(200);
                            }
                        );
                    },
                    "network-access",
                    HealthStatus.Degraded,
                    new[] {"network"});
        }
    }
}