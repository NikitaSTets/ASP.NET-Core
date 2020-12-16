﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ASP.NET_Core_Check.HealthCheck;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ASP.NET_Core_Check.Infrastructure.HostedServices
{
    public class StartupHostedService : IHostedService, IDisposable
    {
        private readonly int _delaySeconds = 15;
        private readonly ILogger _logger;
        private readonly StartupHostedServiceHealthCheck _startupHostedServiceHealthCheck;

        public StartupHostedService(ILogger<StartupHostedService> logger, StartupHostedServiceHealthCheck startupHostedServiceHealthCheck)
        {
            _logger = logger;
            _startupHostedServiceHealthCheck = startupHostedServiceHealthCheck;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Startup Background Service is starting.");

            Task.Run(async () =>
            {
                await Task.Delay(_delaySeconds * 1000, cancellationToken);

                _startupHostedServiceHealthCheck.StartupTaskCompleted = true;

                _logger.LogInformation("Startup Background Service has started.");
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Startup Background Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}