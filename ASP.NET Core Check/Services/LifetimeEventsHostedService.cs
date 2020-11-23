using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ASP.NET_Core_Check.Services
{
    internal class LifetimeEventsHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IHostEnvironment _hostEnvironment;


        public LifetimeEventsHostedService(
            ILogger<LifetimeEventsHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _hostEnvironment = hostEnvironment;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        private void OnStarted()
        {
            var a = _hostEnvironment.EnvironmentName;
            _logger.LogInformation($"{_hostEnvironment.ApplicationName} OnStarted has been called. ");
            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation($"{_hostEnvironment.ApplicationName} OnStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation($"{_hostEnvironment.ApplicationName} OnStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}
