using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tesseract.ApiModel.Jobs;
using Tesseract.Client.Utils;
using Tesseract.Common.Extensions;
using Tesseract.Common.Results;

namespace Tesseract.Client.Proxies
{
    public class JobsProxy : TesseractProxyBase
    {
        internal JobsProxy(TesseractProxyInternalData internalData) : base(internalData)
        {
        }

        public async Task<GetJobListResponse> GetJobList()
        {
            var response = await GenericSendRequestAsync(HttpMethod.Get, "/api/jobs/list");
            return await response.DeserializeAsync<GetJobListResponse>();
        }

        public async Task<GetJobStatusResponse> GetJobStatus(string jobId)
        {
            if (jobId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Get, $"/api/jobs/j/{jobId}");
            return await response.DeserializeAsync<GetJobStatusResponse>(true);
        }

        public async Task<ApiValidationResult> ResumeJob(string jobId)
        {
            if (jobId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/jobs/j/{jobId}/actions/resume");
        }

        public async Task<ApiValidationResult> PauseJob(string jobId)
        {
            if (jobId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/jobs/j/{jobId}/actions/pause");
        }

        public async Task<ApiValidationResult> DrainJob(string jobId)
        {
            if (jobId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/jobs/j/{jobId}/actions/drain");
        }

        public async Task<ApiValidationResult> StopJob(string jobId)
        {
            if (jobId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/jobs/j/{jobId}/actions/stop");
        }

        public async Task<ApiValidationResult> PurgeJobQueue(string jobId)
        {
            if (jobId.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            return await InternalSendRequest(HttpMethod.Put, $"/api/jobs/j/{jobId}/actions/purge-queue");
        }

        public async Task<StartReindexingAllAccountsResponse> StartReindexingAllAccounts()
        {
            var response = await GenericSendRequestAsync(HttpMethod.Post, "/api/jobs/reindex/accounts/all");
            return await response.DeserializeAsync<StartReindexingAllAccountsResponse>();
        }

        public async Task<StartPushingAccountsResponse> StartPushingAllAccounts(
            StartPushingAllAccountsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Post, "/api/jobs/push/accounts/all", request);
            return await response.DeserializeAsync<StartPushingAccountsResponse>();
        }

        public async Task<StartPushingAccountsResponse> StartPushingAccountsByTagNs(
            string tagNs, StartPushingAccountsByTagNsRequest request)
        {
            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Post,
                $"/api/jobs/push/accounts/by/tagns", request);
            return await response.DeserializeAsync<StartPushingAccountsResponse>();
        }

        public async Task<StartPushingAccountsResponse> StartPushingAccountsByTag(
            string tagNs, string tag, StartPushingAccountsByTagRequest request)
        {
            if (tagNs.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tagNs));
            }

            if (tag.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Post,
                $"/api/jobs/push/accounts/by/tag", request);
            return await response.DeserializeAsync<StartPushingAccountsResponse>();
        }

        public async Task<StartPushingAccountsResponse> StartPushingAccountsByQuery(
            StartPushingAccountsByQueryRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await GenericSendRequestAsync(HttpMethod.Post, "/api/jobs/push/accounts/by/query", request);
            return await response.DeserializeAsync<StartPushingAccountsResponse>();
        }
    }
}