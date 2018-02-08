using System.Threading.Tasks;
using Tesseract.ApiModel.General;
using Tesseract.Common.ComposerImposter;
using Tesseract.Common.Results;

namespace Tesseract.Core.Index
{
    [Contract]
    public interface IAccountIndexReader
    {
        Task<long> Count(string tenantId, AccountQuery query);

        Task<AccountQueryResultPage> List(string tenantId, AccountQuery query, int count, string continueFrom);

        Task<AccountQueryScrollPage> StartScroll(string tenantId, AccountQuery accountQuery, int count,
            int timeoutSeconds, int sliceCount, int sliceId);

        Task<AccountQueryScrollPage> ContinueScroll(string scrollId, int timeoutSeconds);
        Task<ApiValidationResult> TerminateScroll(string scrollId);
    }
}