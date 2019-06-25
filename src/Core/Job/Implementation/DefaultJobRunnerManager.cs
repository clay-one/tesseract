using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tesseract.Common.Extensions;
using Tesseract.Core.Job.Runner;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Job.Implementation
{
    public class DefaultJobRunnerManager : IJobRunnerManager
    {
        private readonly IJobStore _jobStore;
        private readonly ConcurrentDictionary<string, IJobRunner> _runners;
        private readonly IServiceProvider _serviceProvider;

        public DefaultJobRunnerManager(IJobStore jobStore, IServiceProvider serviceProvider)
        {
            _jobStore = jobStore;
            _serviceProvider = serviceProvider;
            _runners = new ConcurrentDictionary<string, IJobRunner>();
        }


        public async Task CheckStoreJobs()
        {
            var jobIds = await _jobStore.LoadAllRunnableIdsFromAnyTenant();
            var runnerIds = _runners.Keys.ToList();

            var jobIdsToStart = jobIds.Except(runnerIds);

            foreach (var jobId in jobIdsToStart)
            {
                await CheckHealthOrCreateRunner(jobId);
            }
        }

        public async Task CheckHealthOfAllRunners()
        {
            foreach (var runner in _runners.Values.ToList())
            {
                try
                {
                    if (runner.IsProcessTerminated)
                    {
                        if (await runner.CheckHealth())
                        {
                            RemoveRunnerFromList(runner.JobId);
                        }
                        else
                        {
                            await RestartRunner(runner.JobId);
                        }

                        continue;
                    }

                    if (await runner.CheckHealth())
                    {
                        continue;
                    }

                    await RestartRunner(runner.JobId);
                }
                catch (Exception)
                {
                    // TODO: Log
                }
            }
        }

        public async Task CheckHealthOrCreateRunner(string jobId)
        {
            var runner = await GetOrCreateRunner(jobId);
            if (runner == null)
            {
                return;
            }

            if (!await runner.CheckHealth())
            {
                await RestartRunner(jobId);
            }
        }

        public void StopAllRunners()
        {
            foreach (var runner in _runners.Values.ToList())
            {
                runner.StopRunner();
            }
        }

        public bool IsJobRunnerActive(string jobId)
        {
            return _runners.TryGetValue(jobId, out var runner) && !runner.IsProcessTerminated;
        }

        private async Task<IJobRunner> GetOrCreateRunner(string jobId)
        {
            // Before loading data from store, check if the runner exists in memory
            if (_runners.TryGetValue(jobId, out var jobRunner))
            {
                return jobRunner;
            }

            var jobData = await _jobStore.LoadFromAnyTenant(jobId);
            if (jobData.Status.State < JobState.InProgress || jobData.Status.State >= JobState.Completed)
            {
                return null;
            }

            foreach (var preJobId in jobData.Configuration.PreprocessorJobIds.EmptyIfNull())
            {
                // Make sure preprocessor jobs are running before creating this runner
                await GetOrCreateRunner(preJobId);
            }

            return _runners.GetOrAdd(jobId, s =>
            {
                try
                {
                    var newRunner = BuildNewJobRunner(jobData);
                    newRunner.Initialize(jobData);
                    return newRunner;
                }
                catch (Exception e)
                {
                    // TODO Log error details
                    var newRunner = _serviceProvider.GetService<FaultyJobRunner>()
                        .SetFault($"Fatal exception of type {e.GetType().Name} during runner initialization. " +
                                  $"Message: {e.Message}", e);

                    newRunner.Initialize(jobData);
                    return newRunner;
                }
            });
        }

        private IJobRunner BuildNewJobRunner(JobData jobData)
        {
            var stepType = Type.GetType(jobData.JobStepType);
            if (stepType == null)
            {
                return _serviceProvider.GetService<FaultyJobRunner>()
                    .SetFault($"Type '{jobData.JobStepType}' could not be loaded.");
            }

            if (!stepType.IsSubclassOf(typeof(JobStepBase)))
            {
                return _serviceProvider.GetService<FaultyJobRunner>()
                    .SetFault($"Type '{jobData.JobStepType}' should be a subclass of {nameof(JobStepBase)}.");
            }

            var contract = typeof(IJobRunner<>).MakeGenericType(stepType);
            if (!(_serviceProvider.GetService(contract) is IJobRunner jobRunner))
            {
                return _serviceProvider.GetService<FaultyJobRunner>()
                    .SetFault($"Composer did not return an appropriate result for type '{jobData.JobStepType}'");
            }

            return jobRunner;
        }

        private async Task RestartRunner(string jobId)
        {
            // TODO
        }

        private void RemoveRunnerFromList(string jobId)
        {
            _runners.TryRemove(jobId, out _);
        }
    }
}