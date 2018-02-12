using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComposerCore.Attributes;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.General;
using Tesseract.Common.Extensions;
using Tesseract.Core.JobTypes.AccountIndexing;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;

namespace Tesseract.Core.Logic.Implementation
{
    [Component]
    public class DefaultTaggingLogic : ITaggingLogic
    {
        [ComponentPlug]
        public IAccountStore AccountStore { get; set; }

        [ComponentPlug]
        public IJobQueue<AccountIndexingStep> AccountIndexingQueue { get; set; }

        [ComponentPlug]
        public ICurrentTenantLogic Tenant { get; set; }

        public async Task AddTag(string accountId, string ns, string tag)
        {
            // In case account exists, but the tag is not present
            await AccountStore.SetTagWeightIfTagDoesntExist(Tenant.Id, accountId, ns, tag, 1.0d);

            // In case account doesn't exist at all
            await AccountStore.SetTagWeightIfAccountDoesntExist(Tenant.Id, accountId, ns, tag, 1.0d);

            await AccountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = Tenant.Id,
                AccountId = accountId
            });
        }

        public async Task SetTagWeight(string accountId, string ns, string tag, double weight)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator - we get zero as input and want to compare it to exact zero.
            if (weight <= 0d)
                await AccountStore.RemoveTags(Tenant.Id, accountId, new FqTag {Ns = ns, Tag = tag}.Yield());
            else
                await AccountStore.ChangeAccount(Tenant.Id, accountId, new PatchAccountRequest
                {
                    TagChanges = new List<AccountTagChangeInstruction>
                    {
                        new AccountTagChangeInstruction {TagNs = ns, Tag = tag, Weight = weight}
                    }
                });

            await AccountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = Tenant.Id,
                AccountId = accountId
            });
        }

        public async Task ReplaceTagNs(string accountId, string ns, IEnumerable<KeyValuePair<string, double>> tags)
        {
            await AccountStore.RemoveTagNs(Tenant.Id, accountId, ns);

            if (tags != null)
                await AccountStore.ChangeAccount(Tenant.Id, accountId, new PatchAccountRequest
                {
                    TagChanges = tags
                        .Where(t => t.Value > 0d)
                        .Select(t => new AccountTagChangeInstruction {TagNs = ns, Tag = t.Key, Weight = t.Value})
                        .ToList()
                });

            await AccountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = Tenant.Id,
                AccountId = accountId
            });
        }
    }
}