using System.Threading.Tasks;
using Tesseract.ApiModel.General;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Index;

namespace Tesseract.Core.Logic.Implementation
{
    [Component]
    public class DefaultIndexLogic : IIndexLogic
    {
        [ComponentPlug]
        public IAccountIndexReader AccountIndex { get; set; }

        [ComponentPlug]
        public ICurrentTenantLogic Tenant { get; set; }

        public async Task<long> Count(AccountQuery query)
        {
            return await AccountIndex.Count(Tenant.Id, query);
        }

        public async Task<AccountQueryResultPage> List(AccountQuery query, int count, string continueFrom)
        {
            return await AccountIndex.List(Tenant.Id, query, count, continueFrom);
        }
    }
}