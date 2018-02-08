using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tesseract.ApiModel.Tags;
using Tesseract.Client.Utils;
using Tesseract.Common.Extensions;
using Tesseract.Common.Results;

namespace Tesseract.Client.Proxies
{
    public class TagsProxy : TesseractProxyBase
    {
        internal TagsProxy(TesseractProxyInternalData internalData) : base(internalData)
        {
        }

        public async Task<GetTagNsListResponse> GetTagNsList()
        {
            var response = await GenericSendRequestAsync(HttpMethod.Get, "/api/tags/ns-list");
            return await response.DeserializeAsync<GetTagNsListResponse>();
        }

        public async Task<GetTagNsDefinitionResponse> GetTagNsDefinition(string tagNs)
        {
            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Get, $"/api/tags/ns/{tagNs}");
            return await response.DeserializeAsync<GetTagNsDefinitionResponse>(true);
        }

        public async Task<ApiValidationResult> PutTagNsDefinition(string tagNs, PutTagNsDefinitionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/tags/ns/{tagNs}", request);
        }

        public async Task<ApiValidatedResult<GetTagAccountListResponse>> GetTagAccountList(
            string tagNs, string tag, int count = 50, string continueFrom = "")
        {
            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var url = $"/api/tags/ns/{tagNs}/t/{tag}/accounts/list?count={count}";
            if (!string.IsNullOrWhiteSpace(continueFrom))
            {
                url += $"&continueFrom={continueFrom}";
            }

            return await InternalSendRequest<GetTagAccountListResponse>(HttpMethod.Get, url);
        }

        public async Task<ApiValidationResult> PutTagInAccounts(
            string tagNs, string tag, PutTagInAccountsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/tags/ns/{tagNs}/t/{tag}/accounts/list", request);
        }

        public async Task<ApiValidationResult> DeleteTagFromAccounts(
            string tagNs, string tag, DeleteTagFromAccountsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest(HttpMethod.Delete, $"/api/tags/ns/{tagNs}/t/{tag}/accounts/list", request);
        }

        public async Task<ApiValidationResult> PutAccountWeightsOnTag(
            string tagNs, string tag, PutAccountWeightsOnTagRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/tags/ns/{tagNs}/t/{tag}/accounts", request);
        }

        public async Task<ApiValidationResult> PatchAccountWeightsOnTag(
            string tagNs, string tag, PatchAccountWeightsOnTagRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest(new HttpMethod("PATCH"), $"/api/tags/ns/{tagNs}/t/{tag}/accounts",
                request);
        }

        public async Task<ApiValidatedResult<PatchAccountWeightsOnTagResponse>> PatchAccountWeightsOnTagAndWait(
            string tagNs,
            string tag, PatchAccountWeightsOnTagRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest<PatchAccountWeightsOnTagResponse>(
                new HttpMethod("PATCH"), $"/api/tags/ns/{tagNs}/t/{tag}/accounts/sync", request);
        }
    }
}