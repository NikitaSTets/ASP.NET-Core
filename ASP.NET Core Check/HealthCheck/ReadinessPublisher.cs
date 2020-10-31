using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ASP.NET_Core_Check.HealthCheck
{
    public class ReadinessPublisher : IHealthCheckPublisher
    {
        private readonly ILogger _logger;

        public ReadinessPublisher(ILogger<ReadinessPublisher> logger)
        {
            _logger = logger;
        }


        public Task PublishAsync(HealthReport report,
            CancellationToken cancellationToken)
        {
            if (report.Status == HealthStatus.Healthy)
            {
                _logger.LogInformation("{Timestamp} Readiness Probe Status: {Result}",
                    DateTime.UtcNow, report.Status);
            }
            else
            {
                _logger.LogError("{Timestamp} Readiness Probe Status: {Result}",
                    DateTime.UtcNow, report.Status);
            }

            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }
    }
}
