using System.Threading.Tasks;
using ComposerCore.Attributes;
using Tesseract.ApiModel.Accounts;
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