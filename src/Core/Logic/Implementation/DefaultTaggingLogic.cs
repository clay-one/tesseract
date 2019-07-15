using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.General;
using Tesseract.Common.Extensions;
using Tesseract.Core.JobTypes.AccountIndexing;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;

namespace Tesseract.Core.Logic.Implementation
{
    public class DefaultTaggingLogic : ITaggingLogic
    {
        private readonly IAccountStore _accountStore;
        private readonly IJobQueue<AccountIndexingStep> _accountIndexingQueue;
        private readonly ICurrentTenantLogic _tenant;

        public DefaultTaggingLogic(IAccountStore accountStore, IJobQueue<AccountIndexingStep> queue, ICurrentTenantLogic tenantLogic)
        {
            _accountStore = accountStore;
            _accountIndexingQueue = queue;
            _tenant = tenantLogic;
        }

        public async Task AddTag(string accountId, string ns, string tag)
        {
            // In case account exists, but the tag is not present
            await _accountStore.SetTagWeightIfTagDoesntExist(_tenant.Id, accountId, ns, tag, 1.0d);

            // In case account doesn't exist at all
            await _accountStore.SetTagWeightIfAccountDoesntExist(_tenant.Id, accountId, ns, tag, 1.0d);

            await _accountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = _tenant.Id,
                AccountId = accountId
            });
        }

        public async Task SetTagWeight(string accountId, string ns, string tag, double weight)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator - we get zero as input and want to compare it to exact zero.
            if (weight <= 0d)
                await _accountStore.RemoveTags(_tenant.Id, accountId, new FqTag { Ns = ns, Tag = tag }.Yield());
            else
                await _accountStore.ChangeAccount(_tenant.Id, accountId, new PatchAccountRequest
                {
                    TagChanges = new List<AccountTagChangeInstruction>
                    {
                        new AccountTagChangeInstruction {TagNs = ns, Tag = tag, Weight = weight}
                    }
                });

            await _accountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = _tenant.Id,
                AccountId = accountId
            });
        }

        public async Task ReplaceTagNs(string accountId, string ns, IEnumerable<KeyValuePair<string, double>> tags)
        {
            await _accountStore.RemoveTagNs(_tenant.Id, accountId, ns);

            if (tags != null)
                await _accountStore.ChangeAccount(_tenant.Id, accountId, new PatchAccountRequest
                {
                    TagChanges = tags
                        .Where(t => t.Value > 0d)
                        .Select(t => new AccountTagChangeInstruction { TagNs = ns, Tag = t.Key, Weight = t.Value })
                        .ToList()
                });

            await _accountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = _tenant.Id,
                AccountId = accountId
            });
        }
    }
}