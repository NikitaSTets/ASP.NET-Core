using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ASP.NET_Core_Check.HealthCheck
{
    public class StartupHostedServiceHealthCheck : IHealthCheck
    {
        private volatile bool _startupTaskCompleted;

        public string Name => "slow_dependency_check";

        public bool StartupTaskCompleted
        {
            get => _startupTaskCompleted;
            set => _startupTaskCompleted = value;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StartupTaskCompleted
                ? HealthCheckResult.Healthy("The startup task is finished.")
                : HealthCheckResult.Unhealthy("The startup task is still running."));
        }
    }
}