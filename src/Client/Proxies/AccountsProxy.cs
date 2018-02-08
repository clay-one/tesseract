using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Tesseract.Client.Utils;
using Tesseract.Common.Extensions;
using Tesseract.Common.Results;

namespace Tesseract.Client.Proxies
{
    public class AccountsProxy : TesseractProxyBase
    {
        internal AccountsProxy(TesseractProxyInternalData internalData) : base(internalData)
        {
        }

        public async Task<GetAccountInfoResponse> GetAccountInfo(string accountId)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Get, $"/api/accounts/a/{accountId}");
            return await response.DeserializeAsync<GetAccountInfoResponse>(true);
        }

        public async Task<ApiValidationResult> PutAccountChange(string accountId, PutAccountChangeRequest changes)
        {
            if (changes == null)
            {
                throw new ArgumentNullException(nameof(changes));
            }

            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/accounts/a/{accountId}/changes", changes);
        }

        public async Task<ApiValidationResult> PatchAccount(string accountId, PatchAccountRequest patches)
        {
            if (patches == null)
            {
                throw new ArgumentNullException(nameof(patches));
            }

            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            return await InternalSendRequest(new HttpMethod("PATCH"), $"/api/accounts/a/{accountId}", patches);
        }

        public async Task<ApiValidatedResult<PatchAccountResponse>> PatchAccountAndWait(
            string accountId, PatchAccountRequest patches)
        {
            if (patches == null)
            {
                throw new ArgumentNullException(nameof(patches));
            }

            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            return await InternalSendRequest<PatchAccountResponse>(
                new HttpMethod("PATCH"), $"/api/accounts/a/{accountId}/sync", patches);
        }

        public async Task<GetAllTagsOfAccountResponse> GetAllTagsOfAccount(string accountId)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Get, $"/api/accounts/a/{accountId}/tags");
            return await response.DeserializeAsync<GetAllTagsOfAccountResponse>(true);
        }

        public async Task<GetTagsOfAccountInNsResponse> GetTagsOfAccountInNs(string accountId, string tagNs)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            var response = await GenericSendRequestAsync(
                HttpMethod.Get, $"/api/accounts/a/{accountId}/tags/ns/{tagNs}");
            return await response.DeserializeAsync<GetTagsOfAccountInNsResponse>(true);
        }

        public async Task<ApiValidationResult> PutAndReplaceAccountTagsInNs(string accountId, string tagNs,
            PutAndReplaceAccountTagsInNsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/accounts/a/{accountId}/tags/ns/{tagNs}", request);
        }

        public async Task<ApiValidationResult> PutTagOnAccount(string accountId, string tagNs, string tag)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/accounts/a/{accountId}/tags/ns/{tagNs}/t/{tag}");
        }

        public async Task<ApiValidationResult> PutTagWeightOnAccount(
            string accountId, string tagNs, string tag, double weight)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest(
                HttpMethod.Put, $"/api/accounts/a/{accountId}/tags/ns/{tagNs}/t/{tag}/weight/{weight}");
        }

        public async Task<ApiValidationResult> PatchTagWeightOnAccount(string accountId, string tagNs, string tag,
            double weightDelta)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest(
                new HttpMethod("PATCH"),
                $"/api/accounts/a/{accountId}/tags/ns/{tagNs}/t/{tag}/weight-delta/{weightDelta}");
        }

        public async Task<ApiValidatedResult<PatchTagWeightOnAccountResponse>> PatchTagWeightOnAccountAndWait(
            string accountId, string tagNs, string tag,
            double weightDelta)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest<PatchTagWeightOnAccountResponse>(
                new HttpMethod("PATCH"),
                $"/api/accounts/a/{accountId}/tags/ns/{tagNs}/t/{tag}/weight-delta/{weightDelta}/sync");
        }

        public async Task<ApiValidationResult> DeleteTagNsFromAccount(string accountId, string tagNs)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            return await InternalSendRequest(HttpMethod.Delete, $"/api/accounts/a/{accountId}/tags/ns/{tagNs}");
        }

        public async Task<ApiValidationResult> DeleteTagFromAccount(string accountId, string tagNs, string tag)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return await InternalSendRequest(HttpMethod.Delete, $"/api/accounts/a/{accountId}/tags/ns/{tagNs}/t/{tag}");
        }

        public async Task<GetTagWeightOnAccountResponse> GetTagWeightOnAccount(
            string accountId, string tagNs, string tag)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var response = await GenericSendRequestAsync(
                HttpMethod.Get, $"/api/accounts/a/{accountId}/tags/ns/{tagNs}/t/{tag}");
            return await response.DeserializeAsync<GetTagWeightOnAccountResponse>();
        }

        public Task<ApiValidationResult> PutFieldValueOnAccount(string accountId, string fieldName, double fieldValue)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (fieldName.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            // Placeholder for later
            throw new NotImplementedException();
        }

        public async Task<GetFieldValueOnAccountResponse> GetFieldValueOnAccount(string accountId, string fieldName)
        {
            // todo: correct the Uri pattern
            // todo: GET	/accounts/a/:a/tags/ns/:ns/t/:t	-B, SAFE

            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (fieldName.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Get, string.Empty);
            return await response.DeserializeAsync<GetFieldValueOnAccountResponse>();
        }

        public async Task<ApiValidationResult> MarkAccountForReindexing(string accountId)
        {
            if (accountId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            return await InternalSendRequest(HttpMethod.Delete, $"/api/accounts/a/{accountId}/index");
        }

        public async Task<ApiValidatedResult<GetAccountQueryResultsResponse>> GetAccountQueryResults(
            GetAccountQueryResultsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await InternalSendRequest<GetAccountQueryResultsResponse>(
                HttpMethod.Post, "/api/accounts/query/list", request);
        }
    }
}