using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.General;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Storage
{
    [Contract]
    public interface IAccountStore
    {
        Task<AccountData> LoadAccount(string tenantId, string accountId);
        Task<List<AccountData>> LoadAccounts(string tenantId, IEnumerable<string> accountIds);
        Task<AccountData> ChangeAccount(string tenantId, string accountId, PatchAccountRequest patch);

        Task<AccountData> SetTagWeightIfTagDoesntExist(string tenantId, string accountId, string ns, string tag,
            double weight);

        Task<AccountData> SetTagWeightIfAccountDoesntExist(string tenantId, string accountId, string ns, string tag,
            double weight);

        Task<AccountData> RemoveTags(string tenantId, string accountId, IEnumerable<FqTag> fqTags);
        Task<AccountData> RemoveTagNs(string tenantId, string accountId, string ns);
        Task<AccountData> RemoveTagIfNotPositive(string tenantId, string accountId, string ns, string tag);

        Task<List<string>> FetchAccountIds(int batchSize, string tenantId,
            string lowerBound, bool lowerBoundInclusive, string upperBound, bool upperBoundInclusive);
    }
}