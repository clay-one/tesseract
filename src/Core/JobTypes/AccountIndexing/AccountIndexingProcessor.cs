using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesseract.Core.Index;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.JobTypes.AccountIndexing
{
    public class AccountIndexingProcessor : IJobProcessor<AccountIndexingStep>
    {
        private readonly IAccountStore _accountStore;

        private readonly IAccountIndexWriter _indexWriter;

        private readonly IAccountIndexMapper _indexMapper;

        public AccountIndexingProcessor(IAccountStore accountStore,
            IAccountIndexWriter accountIndexWriter,
            IAccountIndexMapper accountIndexMapper)
        {
            _accountStore = accountStore;
            _indexWriter = accountIndexWriter;
            _indexMapper = accountIndexMapper;
        }

        public void Initialize(JobData jobData)
        {
        }

        public async Task<JobProcessingResult> Process(List<AccountIndexingStep> items)
        {
            if (items == null || !items.Any())
            {
                return new JobProcessingResult();
            }

            var nullCounts = await Task.WhenAll(items.GroupBy(i => i.TenantId).Select(async t =>
            {
                var tenantId = t.Key;

                var data = await _accountStore.LoadAccounts(tenantId, t.Select(ti => ti.AccountId));
                var indexModels = data.Where(d => d != null).Select(d => _indexMapper.MapAccountData(d)).ToList();

                if (indexModels.Any())
                {
                    await _indexWriter.Index(tenantId, indexModels);
                }

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