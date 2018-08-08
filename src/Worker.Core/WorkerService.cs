using System;
using System.Threading;
using System.Threading.Tasks;
using ComposerCore;
using ComposerCore.Implementation;
using Tesseract.Core.Job;

namespace Tesseract.Worker.Core
{
    public class WorkerService
    {
        private IComponentContext _composer;
        private bool _stopping;

        public void Start()
        {
            StartAsync().GetAwaiter().GetResult();
        }

        public void Stop()
        {
            _stopping = true;
            StopAsync().GetAwaiter().GetResult();
        }

        private async Task StartAsync()
        {
            _composer = new ComponentContext();
            //_composer.ProcessApplicationConfiguration();

            // Make sure all static jobs are defined on the database
            foreach (var component in _composer.GetAllComponents<IStaticJob>())
            {
                await component.EnsureJobsDefined();
            }

            await _composer.GetComponent<IJobManager>().CleanupOldJobs();

            // Watch on notification channel, to get notified of job changes immediately
            await _composer.GetComponent<IJobNotification>().StartNotificationTargetThread();

            // Start a runner for ongoing jobs on startup
            await _composer.GetComponent<IJobRunnerManager>().CheckStoreJobs();

            // Start a monitor thread
            new Thread(() =>
            {
                while (true)
                {
                    for (var i = 0; i < 60; i++)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        if (_stopping)
                        {
                            break;
                        }
                    }

                    if (_stopping)
                    {
                        break;
                    }

                    try
                    {
                        _composer.GetComponent<IJobRunnerManager>().CheckStoreJobs().GetAwaiter().GetResult();
                        _composer.GetComponent<IJobRunnerManager>().CheckHealthOfAllRunners().GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        // TODO: Log exception
                    }
                }
            }).Start();
        }

        private async Task StopAsync()
        {
            await _composer.GetComponent<IJobNotification>().StopNotificationTargetThread();

            _composer.GetComponent<IJobRunnerManager>().StopAllRunners();
        }
    }
}