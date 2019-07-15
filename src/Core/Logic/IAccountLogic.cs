using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Logic
{
    public interface IAccountLogic
    {
        Task<AccountData> PatchAccount(string accountId, PatchAccountRequest patch);
        Task QueueForReindex(string accountId);
    }
}