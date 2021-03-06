using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AlfaBot.Core.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;

// ReSharper disable ClassNeverInstantiated.Global

namespace AlfaBot.Host.HealthCheckers
{
    /// <inheritdoc />
    /// <summary>
    /// Custom check fro count in the queue
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class QueueCountHealthCheck : IHealthCheck
    {
        private readonly IQueueService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueCountHealthCheck"/> class.
        /// <param name="service">IQueueService instance</param>
        /// </summary>
        public QueueCountHealthCheck(IQueueService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <inheritdoc />
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var now = new ObjectId(DateTime.Now, 0, 0, 0).CreationTime;

            var countHigh = await _service.HighPriorityCountAsync();
            var countLow = await _service.LowPriorityCountAsync();

            var timeHigh = countHigh > 0 ? (now - _service.HighPriorityTime()).TotalSeconds : 0;
            var timeLow = countLow > 0 ? (now - _service.LowPriorityTime()).TotalSeconds : 0;

            var data = new Dictionary<string, object>
            {
                {"countLow", countLow},
                {"countHigh", countHigh},
                {"timeHigh", timeHigh},
                {"timeLow", timeLow}
            };

            if (countHigh > 100 || countLow > 100 || timeHigh > 60 || timeLow > 60)
            {
                return
                    HealthCheckResult.Unhealthy("Owwwww. There are more transaction in the queues",
                        data: data);
            }

            return HealthCheckResult.Healthy("Everything is OK", data);
        }
    }
}