using System.Threading.Tasks;
using Tesseract.ApiModel.General;
using Tesseract.Core.Index;

namespace Tesseract.Core.Logic.Implementation
{
    public class DefaultIndexLogic : IIndexLogic
    {
        private readonly IAccountIndexReader _accountIndex;
        private readonly ICurrentTenantLogic _tenant;

        public DefaultIndexLogic(IAccountIndexReader accountIndexReader, ICurrentTenantLogic tenantLogic)
        {
            _accountIndex = accountIndexReader;
            _tenant = tenantLogic;
        }

        public async Task<long> Count(AccountQuery query)
        {
            return await _accountIndex.Count(_tenant.Id, query);
        }

        public async Task<AccountQueryResultPage> List(AccountQuery query, int count, string continueFrom)
        {
            return await _accountIndex.List(_tenant.Id, query, count, continueFrom);
        }
    }
}