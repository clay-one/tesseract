using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;
using Tesseract.ApiModel.General;
using Tesseract.ApiModel.Jobs;
using Tesseract.Core.Job;
using Tesseract.Core.JobTypes.FetchAllForReindex;
using Tesseract.Core.JobTypes.FetchFromIndex;
using Tesseract.Core.JobTypes.HttpPush;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Tesseract.Core.Logic.Implementation
{
    public class DefaultJobLogic : IJobLogic
    {
        private readonly ICurrentTenantLogic _tenant;
        private readonly IJobManager _jobManager;
        private readonly IJobStore _jobStore;
        private readonly IServiceProvider _serviceProvider;

        public DefaultJobLogic(IJobStore jobStore, IJobManager jobManager, ICurrentTenantLogic tenantLogic)
        {
            _tenant = tenantLogic;
            _jobManager = jobManager;
            _jobStore = jobStore;
        }

        public async Task<List<JobData>> LoadAllJobs()
        {
            return await _jobStore.LoadAll(_tenant.Id);
        }

        public async Task<JobData> LoadJob(string jobId)
        {
            return await _jobStore.Load(_tenant.Id, jobId);
        }

        public async Task<long> GetQueueLength(string jobId)
        {
            return await _jobManager.GetQueueLength(_tenant.Id, jobId);
        }

        public async Task<string> CreateReindexAllJob()
        {
            var jobId = await _jobManager.CreateNewJobOrUpdateDefinition<FetchForReindexStep>(_tenant.Id,
                "reindex-all",
                configuration: new JobConfigurationData
                {
                    MaxBatchSize = 1,
                    MaxConcurrentBatchesPerWorker = 1,
                    IdleSecondsToCompletion = 15,
                    MaxBlockedSecondsPerCycle = 60,
                    MaxTargetQueueLength = 100000
                });

            var initialStep = new FetchForReindexStep
            {
                TenantId = _tenant.Id,
                RangeStart = null,
                RangeEnd = null,
                LastAccountId = null
            };

            var jobQueue = _serviceProvider.GetService<IJobQueue<FetchForReindexStep>>();

            await jobQueue.Enqueue(initialStep, jobId);

            return jobId;
        }

        public async Task<string> CreateFetchAccountsJob(AccountQuery query, PushBehaviorSpecification behavior,
            string displayName, string targetJobId, long count)
        {
            var sliceCount = 5;

            var parameters = new FetchFromIndexParameters
            {
                TenantId = _tenant.Id,
                TargetJobId = targetJobId,
                Query = query,
                SliceCount = sliceCount,
                MaxBatchSize = behavior?.MaxBatchSize ?? 1
            };

            var jobId = await _jobManager.CreateNewJobOrUpdateDefinition<FetchFromIndexStep>(_tenant.Id,
                displayName,
                configuration: new JobConfigurationData
                {
                    MaxBatchSize = 1,
                    MaxConcurrentBatchesPerWorker = 2,

                    // Expire after a day, if not specified when
                    ExpiresAt = DateTime.UtcNow.AddSeconds(behavior?.ExpireJobAfterSeconds ?? 24 * 60 * 60),

                    IdleSecondsToCompletion = 60,
                    MaxBlockedSecondsPerCycle = 60,

                    // Try to keep 10 seconds worth of items in queue, but
                    // at least 5k (to avoid small batches of processing and spinning) and
                    // at most 100k (to avoid too much memory usage in queue)
                    MaxTargetQueueLength = Math.Max(5_000, Math.Min(100_000,
                        (int)((behavior?.AccountBatchesPerSecond ?? 10_000d) * 10))),

                    Parameters = parameters.ToJson()
                });

            await _jobManager.AddPredecessor(_tenant.Id, targetJobId, jobId);

            var initialSteps = Enumerable.Range(0, sliceCount).Select(sliceId =>
                new FetchFromIndexStep
                {
                    ScrollId = null,
                    SliceId = sliceId,
                    Sequence = 0
                });

            var jobQueue = _serviceProvider.GetService<IJobQueue<FetchFromIndexStep>>();

            await jobQueue.EnqueueBatch(initialSteps, jobId);

            return jobId;
        }

        public async Task<string> CreatePushAccountsJob(PushBehaviorSpecification behavior,
            PushTargetSpecification target, string displayName, long estimatedCount)
        {
            if (target.Http != null)
                return await CreateHttpPushAccountsJob(behavior, target.Http, displayName, estimatedCount);

            if (target.Kafka != null) return await CreateKafkaPushAccountsJob(behavior, target.Kafka);

            if (target.Redis != null) return await CreateRedisPushAccountsJob(behavior, target.Redis);

            throw new InvalidOperationException("Unknown target type. All target fields are null.");
        }

        private async Task<string> CreateHttpPushAccountsJob(PushBehaviorSpecification behavior,
            HttpPushTargetSpecification target, string displayName, long estimatedCount)
        {
            var parameters = new HttpPushParameters
            {
                FieldValuesToInclude = behavior?.FieldValuesToInclude,
                TagWeightsToInclude = behavior?.TagWeightsToInclude,
                Url = target.Url,
                IgnoreHttpErrors = target.IgnoreHttpErrors,
                MaxInstantRetries = target.MaxInstantRetries ?? 0,
                MaxDelayedRetries = target.MaxDelayedRetries ?? 0,
                RetryDelaySeconds = target.RetryDelaySeconds ?? 0
            };

            var jobId = await _jobManager.CreateNewJobOrUpdateDefinition<HttpPushStep>(_tenant.Id,
                displayName,
                configuration: new JobConfigurationData
                {
                    MaxBatchSize = Math.Min(10, target.HttpConcurrencyLevel ?? 10),
                    MaxConcurrentBatchesPerWorker = target.HttpConcurrencyLevel ?? 50,
                    ThrottledItemsPerSecond = behavior?.AccountBatchesPerSecond,

                    // Half-a-second worth of burst
                    ThrottledMaxBurstSize = behavior?.AccountBatchesPerSecond != null
                        ? (int?)Math.Max(1, (int)(behavior.AccountBatchesPerSecond / 2d))
                        : null,

                    // Expire after a day, if not specified when
                    ExpiresAt = DateTime.UtcNow.AddSeconds(behavior?.ExpireJobAfterSeconds ?? 24 * 60 * 60),

                    IdleSecondsToCompletion = 60,
                    MaxBlockedSecondsPerCycle = 60,
                    Parameters = parameters.ToJson(),
                    PreprocessorJobIds = new List<string>()
                });

            return jobId;
        }

        private Task<string> CreateKafkaPushAccountsJob(PushBehaviorSpecification behavior,
            KafkaPushTargetSpecification target)
        {
            throw new NotImplementedException();
        }

        private Task<string> CreateRedisPushAccountsJob(PushBehaviorSpecification behavior,
            RedisPushTargetSpecification target)
        {
            throw new NotImplementedException();
        }
    }
}