using System.Threading.Tasks;
using ComposerCore.Attributes;
using Tesseract.ApiModel.General;
using Tesseract.Core.Index;

namespace Tesseract.Core.Logic
{
    [Contract]
    public interface IIndexLogic
    {
        Task<long> Count(AccountQuery query);

        Task<AccountQueryResultPage> List(AccountQuery query, int count, string continueFrom);
    }
}