using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tesseract.Core.Job;

namespace Tesseract.Worker.Services
{
    public class WorkerService : IHostedService
    {
        private readonly IJobManager _jobManager;
        private readonly IJobNotification _jobNotification;
        private readonly IJobRunnerManager _jobRunnerManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WorkerService> _logger;

        public WorkerService(IJobManager jobManager,
            IJobNotification jobNotification,
            IJobRunnerManager jobRunnerManager,
            IServiceProvider serviceProvider,
            ILogger<WorkerService> logger)
        {
            _jobManager = jobManager;
            _jobNotification = jobNotification;
            _jobRunnerManager = jobRunnerManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker Background Service is starting.");


            _logger.LogInformation("Ensuring static jobs");
            foreach (var staticJobService in _serviceProvider.GetServices<IStaticJob>())
            {
                _logger.LogInformation($"Ensuring static job: {staticJobService}");
                await staticJobService.EnsureJobsDefined();
            }

            _logger.LogInformation("Cleaning up old jobs");
            await _jobManager.CleanupOldJobs();

            _logger.LogInformation("Starting notification target channel");
            await _jobNotification.StartNotificationTargetThread();

            _logger.LogInformation("Checking stored jobs");
            await _jobRunnerManager.CheckStoreJobs();


            _logger.LogInformation("worker service started successfully");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("stopping worker service");


            _logger.LogInformation("stopping notification target channel");
            await _jobNotification.StopNotificationTargetThread();

            _logger.LogInformation("stopping runners");
            _jobRunnerManager.StopAllRunners();


            _logger.LogInformation("worker service stopped completely");

        }
    }
}