using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tesseract.ApiModel.Statistics;
using Tesseract.Common.Extensions;
using Tesseract.Common.Results;

namespace Tesseract.Client.Proxies
{
    public class StatisticsProxy : TesseractProxyBase
    {
        internal StatisticsProxy(TesseractProxyInternalData internalData) : base(internalData)
        {
        }

        public async Task<ApiValidatedResult<GetCountOfAllAccountsResponse>> GetCountOfAllAccounts()
        {
            return await InternalSendRequest<GetCountOfAllAccountsResponse>(HttpMethod.Get, "/api/accounts/count");
        }

        public async Task<ApiValidatedResult<GetTagNsAccountCountResponse>> GetTagNsAccountCount(string tagNs)
        {
            // GET	/tags/ns/:ns/accounts/count	-B, SAFE	Return number of accounts tagged in the specified namespace

            if (tagNs.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(tagNs));

            return await InternalSendRequest<GetTagNsAccountCountResponse>(
                HttpMethod.Get, $"/api/tags/ns/{tagNs}/accounts/count");
        }

        public async Task<ApiValidatedResult<GetTagAccountCountResponse>> GetTagAccountCount(string tagNs, string tag)
        {
            // GET	/tags/ns/:ns/t/:t/accounts/count	-B, SAFE	Return number of accounts that include the specified tag

            if (tagNs.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(tagNs));
            if (tag.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(tag));

            return await InternalSendRequest<GetTagAccountCountResponse>(
                HttpMethod.Get, $"/api/tags/ns/{tagNs}/t/{tag}/accounts/count");
        }

        
        public async Task<ApiValidatedResult<GetAccountQueryResultCountResponse>> GetAccountQueryResultCount(
            GetAccountQueryResultCountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await InternalSendRequest<GetAccountQueryResultCountResponse>(
                HttpMethod.Post, "/api/accounts/query/count", request);
        }
   }
}