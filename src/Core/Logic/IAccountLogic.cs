using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Logic
{
    [Contract]
    public interface IAccountLogic
    {
        Task<AccountData> PatchAccount(string accountId, PatchAccountRequest patch);
        Task QueueForReindex(string accountId);
    }
}