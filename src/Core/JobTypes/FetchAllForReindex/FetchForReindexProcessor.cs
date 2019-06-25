using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Tesseract.Common.Extensions;
using Tesseract.Core.JobTypes.AccountIndexing;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;
using Tesseract.Core.Utility;

namespace Tesseract.Core.JobTypes.FetchAllForReindex
{
    public class FetchForReindexProcessor : IJobProcessor<FetchForReindexStep>
    {
        private const int BatchSize = 500;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<char> _accountIdRangeChars =
            "02468acegikmoqsuwyACEGIKMOQSUWY_-~:"
                .OrderBy(c => c).ToList();

        private string _jobId;

        public FetchForReindexProcessor(IAccountStore accountStore, IJobQueue<AccountIndexingStep> indexQueue,
            IJobQueue<FetchForReindexStep> fetchQueue)
        {
            _accountStore = accountStore;
            _indexQueue = indexQueue;
            _fetchQueue = fetchQueue;
        }

        private readonly IAccountStore _accountStore;

        private readonly IJobQueue<AccountIndexingStep> _indexQueue;

        private readonly IJobQueue<FetchForReindexStep> _fetchQueue;

        public void Initialize(JobData jobData)
        {
            _jobId = jobData.JobId;
        }

        public async Task<JobProcessingResult> Process(List<FetchForReindexStep> items)
        {
            return JobProcessingResult.Combine(
                await Task.WhenAll(items.Select(ProcessOne).ToArray()));
        }

        public Task<long> GetTargetQueueLength()
        {
            return _indexQueue.GetQueueLength();
        }

        public async Task<JobProcessingResult> ProcessOne(FetchForReindexStep item)
        {
            Logger.Debug($"ProcessOne: S='{item.RangeStart ?? ""}', " +
                         $"E='{item.RangeEnd ?? ""}', A='{item.LastAccountId ?? ""}'");

            var result = new JobProcessingResult();

            var accountIds = await _accountStore.FetchAccountIds(
                BatchSize,
                item.TenantId,
                item.LastAccountId ?? item.RangeStart,
                item.LastAccountId == null,
                item.RangeEnd,
                false);

            if (accountIds.Any())
            {
                Logger.Debug($"    => Fetched {accountIds.Count} items, " +
                             $"from {accountIds[0]} to {accountIds[accountIds.Count - 1]}");
            }
            else
            {
                Logger.Debug("    => Fetched nothing :(");
            }

            if (accountIds.Count >= BatchSize)
            {
                var subRanges = CalculateRangeBreakdowns(item, accountIds[accountIds.Count - 1]);
                await _fetchQueue.EnqueueBatch(subRanges, _jobId);

                if (Logger.IsDebugEnabled)
                {
                    subRanges.ForEach(sr =>
                    {
                        Logger.Debug($"    +  S='{sr.RangeStart ?? ""}', " +
                                     $"E='{sr.RangeEnd ?? ""}', A='{sr.LastAccountId ?? ""}'");
                    });
                }

                result.ItemsRequeued += subRanges.Count;
            }

            await _indexQueue.EnqueueBatch(accountIds.Select(aid => new AccountIndexingStep
            {
                TenantId = item.TenantId,
                AccountId = aid
            }));

            result.ItemsGeneratedForTargetQueue += accountIds.Count;
            return result;
        }

        private List<FetchForReindexStep> CalculateRangeBreakdowns(
            FetchForReindexStep currentItem, string lastFetchedAccountId)
        {
            var currentStartLength = currentItem.RangeStart?.Length ?? 0;
            var currentEndLength = currentItem.RangeEnd?.Length ?? 0;

            if (currentEndLength > currentStartLength)
            {
                // In this case, the range cannot be broken any further. So just queue an
                // item to continue fetching this range from where the previous batch left off.
                return new FetchForReindexStep
                {
                    TenantId = currentItem.TenantId,
                    RangeStart = currentItem.RangeStart,
                    RangeEnd = currentItem.RangeEnd,
                    LastAccountId = lastFetchedAccountId
                }.Yield().ToList();
            }

            var result = new List<FetchForReindexStep>();

            for (var i = 0; i < _accountIdRangeChars.Count; i++)
            {
                var newItem = new FetchForReindexStep
                {
                    TenantId = currentItem.TenantId,
                    RangeStart = i == 0
                        ? currentItem.RangeStart
                        : (currentItem.RangeStart ?? "") + _accountIdRangeChars[i - 1],
                    RangeEnd = (currentItem.RangeStart ?? "") + _accountIdRangeChars[i]
                };

                // Check if the range is not already covered, and add it to the result if so.
                AddIfRangeIsNeeded(result, newItem, lastFetchedAccountId);
            }

            var lastItem = new FetchForReindexStep
            {
                TenantId = currentItem.TenantId,
                RangeStart = (currentItem.RangeStart ?? "") + _accountIdRangeChars[_accountIdRangeChars.Count - 1],
                RangeEnd = currentItem.RangeEnd
            };

            AddIfRangeIsNeeded(result, lastItem, lastFetchedAccountId);

            // Shuffle the result to distribute load to storage shards evenly
            if (result.Count > 1)
            {
                result.Shuffle();
            }

            return result;
        }

        private void AddIfRangeIsNeeded(List<FetchForReindexStep> list,
            FetchForReindexStep item, string lastFetchedAccountId)
        {
            // If the last fetched account is above this range, it means that the range is already
            // fetched, and there's no need to queue this range again. 
            if (IsAboveRange(lastFetchedAccountId, item))
            {
                return;
            }

            // If the last fetched account is within this range, it means that the range is
            // partially fetched and should be fetched further. So, queue an item with the
            // last fetched account id specified.
            if (IsInRange(lastFetchedAccountId, item))
            {
                item.LastAccountId = lastFetchedAccountId;
            }

            // Otherwise the whole range is untouched, so queue the new range without any
            // limits on the account id.

            list.Add(item);
        }

        private bool IsInRange(string accountId, FetchForReindexStep item)
        {
            if (accountId == null)
            {
                return false;
            }

            if (item.RangeStart == null || string.CompareOrdinal(accountId, item.RangeStart) > 0)
            {
                if (item.RangeEnd == null || string.CompareOrdinal(accountId, item.RangeEnd) < 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsAboveRange(string accountId, FetchForReindexStep item)
        {
            if (accountId == null)
            {
                return false;
            }

            return item.RangeEnd != null && string.CompareOrdinal(accountId, item.RangeEnd) > 0;
        }
    }
}