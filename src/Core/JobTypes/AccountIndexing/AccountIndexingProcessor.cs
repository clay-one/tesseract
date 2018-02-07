using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Index;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.JobTypes.AccountIndexing
{
    [Component]
    [ComponentCache(null)]
    public class AccountIndexingProcessor : IJobProcessor<AccountIndexingStep>
    {
        [ComponentPlug]
        public IAccountStore AccountStore { get; set; }

        [ComponentPlug]
        public IAccountIndexWriter IndexWriter { get; set; }

        [ComponentPlug]
        public IAccountIndexMapper IndexMapper { get; set; }

        public void Initialize(JobData jobData)
        {
        }

        public async Task<JobProcessingResult> Process(List<AccountIndexingStep> items)
        {
            if (items == null || !items.Any())
                return new JobProcessingResult();

            var nullCounts = await Task.WhenAll(items.GroupBy(i => i.TenantId).Select(async t =>
            {
                var tenantId = t.Key;
                
                var data = await AccountStore.LoadAccounts(tenantId, t.Select(ti => ti.AccountId));
                var indexModels = data.Where(d => d != null).Select(d => IndexMapper.MapAccountData(d)).ToList();

                if (indexModels.Any())
                    await IndexWriter.Index(tenantId, indexModels);

                return data.Count(d => d == null);
            }));

            return new JobProcessingResult
            {
                ItemsFailed = nullCounts.Sum()
            };
        }

        public Task<long> GetTargetQueueLength()
        {
            return Task.FromResult(0L);
        }
    }
}