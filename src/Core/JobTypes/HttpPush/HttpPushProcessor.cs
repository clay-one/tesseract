using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using Tesseract.ApiModel.General;
using Tesseract.ApiModel.Push;
using Tesseract.Common.ComposerImposter;
using Tesseract.Common.Extensions;
using Tesseract.Core.JobTypes.Base;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;
using Tesseract.Core.Utility;

namespace Tesseract.Core.JobTypes.HttpPush
{
    [Component]
    [ComponentCache(null)]
    public class HttpPushProcessor : IJobProcessor<HttpPushStep>
    {
        private HttpClient _client;
        private string _jobId;
        private HttpPushParameters _parameters;
        private string _tenantId;

        [ComponentPlug]
        public IJobQueue<PushStepBase> PushQueue { get; set; }

        [ComponentPlug]
        public IAccountStore AccountStore { get; set; }

        public void Initialize(JobData jobData)
        {
            _tenantId = jobData.TenantId;
            _jobId = jobData.JobId;
            _client = new HttpClient();

            var parametersString = jobData.Configuration?.Parameters;
            if (!string.IsNullOrWhiteSpace(parametersString))
            {
                _parameters = parametersString.FromJson<HttpPushParameters>();
            }
        }


        public async Task<JobProcessingResult> Process(List<HttpPushStep> items)
        {
            return JobProcessingResult.Combine(
                await Task.WhenAll(items.Select(ProcessOne).ToArray()));
        }

        public Task<long> GetTargetQueueLength()
        {
            return Task.FromResult(0L);
        }

        public async Task<JobProcessingResult> ProcessOne(HttpPushStep step)
        {
            if (step.LastAttemptTime.HasValue)
            {
                var secondsSinceLastAttempt = (DateTime.UtcNow - step.LastAttemptTime.Value).TotalSeconds;
                if (secondsSinceLastAttempt < _parameters.RetryDelaySeconds)
                {
                    await PushQueue.Enqueue(step, _jobId);
                    await Task.Delay(1000);

                    return new JobProcessingResult {ItemsRequeued = 1};
                }
            }

            var pushedObject = await PreparePushedObject(step.AccountIds);

            var failureMessage = "";
            for (var i = 0; i <= _parameters.MaxInstantRetries; i++)
            {
                failureMessage = await TrySendHttpRequest(pushedObject);

                if (failureMessage == null)
                {
                    return new JobProcessingResult();
                }
            }

            // Instant retries are exhausted. Report failure, and re-queue if more retries are desired

            var result = new JobProcessingResult();
            result.FailureMessages = new[] {failureMessage};
            result.ItemsFailed++;

            step.LastAttemptTime = DateTime.UtcNow;
            step.NumberOfAttempts++;

            if (step.NumberOfAttempts > _parameters.MaxDelayedRetries)
            {
                return result;
            }

            await PushQueue.Enqueue(step, _jobId);
            result.ItemsRequeued++;

            return result;
        }

        private async Task<PushedAccountInfoBatch> PreparePushedObject(List<string> accountIds)
        {
            if (!_parameters.TagWeightsToInclude.SafeAny() && !_parameters.FieldValuesToInclude.SafeAny())
            {
                return new PushedAccountInfoBatch
                {
                    Accounts = accountIds.Select(aid => new PushedAccountInfo {AccountId = aid}).ToList()
                };
            }

            var accountData = await AccountStore.LoadAccounts(_tenantId, accountIds);
            return new PushedAccountInfoBatch
            {
                Accounts = accountData.Select(MapAccountInfo).ToList()
            };
        }

        private PushedAccountInfo MapAccountInfo(AccountData data)
        {
            var result = new PushedAccountInfo {AccountId = data.AccountId};

            if (_parameters.TagWeightsToInclude.SafeAny())
            {
                result.TagWeights = _parameters.TagWeightsToInclude.Select(t => new FqTagWithWeight
                {
                    Ns = t.Ns,
                    Tag = t.Tag,
                    Weight = data.GetTagWeight(t.Ns, t.Tag)
                }).ToList();
            }

            if (_parameters.FieldValuesToInclude.SafeAny())
            {
                result.FieldValues = _parameters.FieldValuesToInclude.Select(f => new FieldNameWithValue
                {
                    Name = f,
                    Value = data.GetFieldValue(f)
                }).ToList();
            }

            return result;
        }

        private async Task<string> TrySendHttpRequest(PushedAccountInfoBatch pushedObject)
        {
            try
            {
                var payload = new HttpRequestMessage(HttpMethod.Post, _parameters.Url)
                {
                    Content = new StringContent(pushedObject.ToJson(), Encoding.UTF8, "application/json")
                };

                var sendResult = await _client.SendAsync(payload);
                if (sendResult.IsSuccessStatusCode || _parameters.IgnoreHttpErrors)
                {
                    return null;
                }

                return sendResult.StatusCode + " " + sendResult.ReasonPhrase;
            }
            catch (HttpRequestException e)
            {
                return $"Exception: {e.Message}";
            }
        }
    }
}