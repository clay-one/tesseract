using System.Threading.Tasks;
using Tesseract.ApiModel.General;
using Tesseract.Common.ComposerImposter;
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