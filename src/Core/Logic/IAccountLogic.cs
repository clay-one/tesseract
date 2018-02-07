using System.Threading.Tasks;
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