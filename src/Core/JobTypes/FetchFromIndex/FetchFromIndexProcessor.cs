using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using ServiceStack;
using Tesseract.Core.Index;
using Tesseract.Core.JobTypes.Base;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.JobTypes.FetchFromIndex
{
    public class FetchFromIndexProcessor : IJobProcessor<FetchFromIndexStep>
    {
        private const int MaxFetchSize = 2500;

        private readonly IJobQueue<FetchFromIndexStep> _fetchQueue;

        private readonly IAccountIndexReader _index;

        private readonly IJobQueue<PushStepBase> _pushQueue;
        private int _fetchSize;
        private string _jobId;

        private FetchFromIndexParameters _parameters;

        public FetchFromIndexProcessor(IAccountIndexReader accountIndexReader, IJobQueue<PushStepBase> pushQueue,
            IJobQueue<FetchFromIndexStep> fetchQueue)
        {
            _index = accountIndexReader;
            _fetchQueue = fetchQueue;
            _pushQueue = pushQueue;
        }

        public void Initialize(JobData jobData)
        {
            _jobId = jobData.JobId;

            var parametersString = jobData.Configuration?.Parameters;
            if (!string.IsNullOrWhiteSpace(parametersString))
            {
                _parameters = parametersString.FromJson<FetchFromIndexParameters>();
            }

            _fetchSize = MaxFetchSize - MaxFetchSize % _parameters.MaxBatchSize;
        }

        public async Task<JobProcessingResult> Process(List<FetchFromIndexStep> items)
        {
            return JobProcessingResult.Combine(
                await Task.WhenAll(items.Select(ProcessOne).ToArray()));
        }

        public async Task<long> GetTargetQueueLength()
        {
            return await _pushQueue.GetQueueLength(_parameters.TargetJobId);
        }

        public async Task<JobProcessingResult> ProcessOne(FetchFromIndexStep step)
        {
            var result = new JobProcessingResult();
            AccountQueryScrollPage searchResult;

            try
            {
                searchResult = await StartOrContinueScroll(step);
            }
            catch (Exception e)
            {
                result.FailureMessages = new[] {$"Sequence {step.Sequence} of slice {step.SliceId} threw {e.Message}"};
                result.ItemsFailed++;
                return result;
            }

            if (string.IsNullOrWhiteSpace(searchResult.ScrollId))
            {
                result.FailureMessages = new[]
                {
                    $"Sequence {step.Sequence} of slice {step.SliceId} didn't return any" +
                    "ScrollId to continue with."
                };
                result.ItemsFailed++;
                return result;
            }

            if (searchResult.AccountIds.Count > 0)
            {
                var targetSteps = searchResult.AccountIds.Batch(
                    _parameters.MaxBatchSize,
                    aids => new PushStepBase {AccountIds = searchResult.AccountIds}).ToList();

                var nextFetchStep = new FetchFromIndexStep
                {
                    ScrollId = searchResult.ScrollId,
                    SliceId = step.SliceId,
                    Sequence = step.Sequence + 1
                };

                await _pushQueue.EnqueueBatch(targetSteps, _parameters.TargetJobId);
                await _fetchQueue.Enqueue(nextFetchStep, _jobId);

                result.ItemsGeneratedForTargetQueue += targetSteps.Count;
                result.ItemsRequeued++;

                return result;
            }

            try
            {
                await _index.TerminateScroll(searchResult.ScrollId);
            }
            catch (Exception e)
            {
                result.FailureMessages = new[]
                {
                    $"Termination of slice {step.SliceId} at sequence {step.Sequence} " +
                    $"threw {e.Message}"
                };
                result.ItemsFailed++;
            }

            return result;
        }

        private async Task<AccountQueryScrollPage> StartOrContinueScroll(FetchFromIndexStep step)
        {
            AccountQueryScrollPage searchResult;

            if (string.IsNullOrWhiteSpace(step.ScrollId))
            {
                searchResult = await _index.StartScroll(_parameters.TenantId, _parameters.Query, _fetchSize,
                    24 * 60 * 60, _parameters.SliceCount, step.SliceId);
            }
            else
            {
                searchResult = await _index.ContinueScroll(step.ScrollId, 24 * 60 * 60);
            }

            return searchResult;
        }
    }
}