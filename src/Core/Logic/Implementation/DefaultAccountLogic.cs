using System.Linq;
using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.General;
using Tesseract.Common.Extensions;
using Tesseract.Core.JobTypes.AccountIndexing;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Logic.Implementation
{
    public class DefaultAccountLogic : IAccountLogic
    {
        private readonly IAccountStore _accountStore;
        private readonly IJobQueue<AccountIndexingStep> _accountIndexingQueue;
        private readonly ICurrentTenantLogic _tenant;

        public DefaultAccountLogic(IAccountStore accountStore, IJobQueue<AccountIndexingStep> accountIndexingQueue, ICurrentTenantLogic tenantLogic)
        {
            _accountStore = accountStore;
            _accountIndexingQueue = accountIndexingQueue;
            _tenant = tenantLogic;
        }

        public async Task<AccountData> PatchAccount(string accountId, PatchAccountRequest patch)
        {
            var removedTags = patch.TagChanges?.Where(tc => tc.Weight <= 0d).ToList();
            var maybeRemovedTags = patch.TagPatches?.Where(tp => tp.WeightDelta < 0d).ToList();

            var accountInfo = await _accountStore.ChangeAccount(_tenant.Id, accountId, new PatchAccountRequest
            {
                TagChanges = patch.TagChanges?.Where(tc => tc.Weight > 0d).ToList(),
                TagPatches = patch.TagPatches,
                FieldChanges = patch.FieldChanges,
                FieldPatches = patch.FieldPatches
            });

            if (removedTags.SafeAny())
                accountInfo = await _accountStore.RemoveTags(_tenant.Id, accountId,
                    removedTags?.Select(t => new FqTag { Ns = t.TagNs, Tag = t.Tag }).ToList());

            await Task.WhenAll(maybeRemovedTags.EmptyIfNull().Select(async t =>
            {
                accountInfo = await _accountStore.RemoveTagIfNotPositive(_tenant.Id, accountId, t.TagNs, t.Tag);
            }));

            await _accountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = _tenant.Id,
                AccountId = accountId
            });

            return accountInfo;
        }

        public async Task QueueForReindex(string accountId)
        {
            await _accountIndexingQueue.Enqueue(new AccountIndexingStep
            {
                TenantId = _tenant.Id,
                AccountId = accountId
            });
        }
    }
}