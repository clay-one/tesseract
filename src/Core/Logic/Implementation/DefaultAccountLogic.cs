using System.Linq;
using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.General;
using Tesseract.Common.ComposerImposter;
using Tesseract.Common.Extensions;
using Tesseract.Core.JobTypes.AccountIndexing;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Logic.Implementation
{
    [Component]
    public class DefaultAccountLogic : IAccountLogic
    {
        [ComponentPlug]
        public IAccountStore AccountStore { get; set; }

        [ComponentPlug]
        public IJobQueue<AccountIndexingStep> AccountIndexingQueue { get; set; }

        [ComponentPlug]
        public ICurrentTenantLogic Tenant { get; set; }

        public async Task<AccountData> PatchAccount(string accountId, PatchAccountRequest patch)
        {
            var removedTags = patch.TagChanges?.Where(tc => tc.Weight <= 0d).ToList();
            var maybeRemovedTags = patch.TagPatches?.Where(tp => tp.WeightDelta < 0d).ToList();

            var accountInfo = await AccountStore.ChangeAccount(Tenant.Id, accountId, new PatchAccountRequest
            {
                TagChanges = patch.TagChanges?.Where(tc => tc.Weight > 0d).ToList(),
                TagPatches = patch.TagPatches,
                FieldChanges = patch.FieldChanges,
                FieldPatches = patch.FieldPatches
            });

            if (removedTags.SafeAny())
            {
                accountInfo = await AccountStore.RemoveTags(Tenant.Id, accountId,
                    removedTags?.Select(t => new FqTag {Ns = t.TagNs, Tag = t.Tag}).ToList());
            }

            await Task.WhenAll(maybeRemovedTags.EmptyIfNull().Select(async t =>
            {
                accountInfo = await AccountStore.RemoveTagIfNotPositive(Tenant.Id, accountId, t.TagNs, t.Tag);
            }));

            await AccountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = Tenant.Id,
                AccountId = accountId
            });

            return accountInfo;
        }

        public async Task QueueForReindex(string accountId)
        {
            await AccountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = Tenant.Id,
                AccountId = accountId
            });
        }
    }
}