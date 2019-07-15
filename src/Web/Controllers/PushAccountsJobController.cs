using System.Collections.Generic;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.General;
using Tesseract.ApiModel.Jobs;
using Tesseract.Common.Results;
using Tesseract.Core;
using Tesseract.Core.Job;
using Tesseract.Core.Logic;

namespace Appson.Tesseract.Web.Controllers
{
    public class PushAccountsJobController : ApiControllerBase
    {
        // POST	/jobs/push/accounts/all	+B, JOB	Starts pushing list of all known accounts to HTTP/Redis/Kafka target
        // POST	/jobs/push/accounts/by/query	+B, JOB Runs a query on accounts and starts pushing the resulting accounts to HTTP/Redis/Kafka target
        // POST	/jobs/push/accounts/by/tags/ns/:ns	+B, JOB Starts pushing list of all accounts tagged in a specific namespace to HTTP/Redis/Kafka target
        // POST	/jobs/push/accounts/by/tags/ns/:ns/t/:t	+B, JOB Starts pushing list of all accounts tagged with a specific tag to HTTP/Redis/Kafka target


        private readonly IInputValidationLogic _validation;
        private readonly IJobLogic _jobLogic;
        private readonly IJobManager _jobManager;
        private readonly IIndexLogic _indexLogic;
        private readonly ICurrentTenantLogic _tenant;

        public PushAccountsJobController(IInputValidationLogic inputValidationLogic,
            IJobLogic jobLogic,
            IJobManager jobManager,
            IIndexLogic indexLogic,
            ICurrentTenantLogic currentTenantLogic)
        {
            _validation = inputValidationLogic;
            _jobLogic = jobLogic;
            _jobManager = jobManager;
            _indexLogic = indexLogic;
            _tenant = currentTenantLogic;
        }

        /// <summary>
        /// POST
        /// /jobs/push/accounts/all
        /// +B, JOB
        /// Starts pushing list of all known accounts to HTTP/Redis/Kafka target
        /// </summary>
        [HttpPost("jobs/push/accounts/all")]
        public async Task<IActionResult> StartPushingAllAccounts(StartPushingAllAccountsRequest input)
        {
            var validationResult = ValidateForStartPushingAllAccounts(input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var query = new AccountQuery();

            var estimatedCount = await _indexLogic.Count(query);
            if (estimatedCount <= 0)
                return Ok(new StartPushingAccountsResponse { EstimatedNumberOfAccounts = 0 });

            var pushJobId = await _jobLogic.CreatePushAccountsJob(
                input.Behavior, input.Target, (input.JobDisplayName ?? "job") + "_push", estimatedCount);
            var fetchJobId = await _jobLogic.CreateFetchAccountsJob(
                query, input.Behavior, (input.JobDisplayName ?? "job") + "_fetch", pushJobId, estimatedCount);

            await _jobManager.StartJob(_tenant.Id, fetchJobId);
            await _jobManager.StartJob(_tenant.Id, pushJobId);

            return Ok(new StartPushingAccountsResponse
            {
                FetchJobId = fetchJobId,
                PushJobId = pushJobId,
                EstimatedNumberOfAccounts = estimatedCount
            });
        }

        /// <summary>
        /// POST
        /// /jobs/push/accounts/by/query
        /// +B, JOB
        /// Runs a query on accounts and starts pushing the resulting accounts to HTTP/Redis/Kafka target
        /// </summary>
        [HttpPost("jobs/push/accounts/by/query")]
        public async Task<IActionResult> StartPushingAccountsByQuery(StartPushingAccountsByQueryRequest input)
        {
            var validationResult = ValidateForStartPushingAccountsByQuery(input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var estimatedCount = await _indexLogic.Count(input.Query);
            if (estimatedCount <= 0)
                return Ok(new StartPushingAccountsResponse { EstimatedNumberOfAccounts = 0 });

            var pushJobId = await _jobLogic.CreatePushAccountsJob(
                input.Behavior, input.Target, (input.JobDisplayName ?? "job") + "_push", estimatedCount);
            var fetchJobId = await _jobLogic.CreateFetchAccountsJob(
                input.Query, input.Behavior, (input.JobDisplayName ?? "job") + "_fetch", pushJobId, estimatedCount);

            await _jobManager.StartJob(_tenant.Id, fetchJobId);
            await _jobManager.StartJob(_tenant.Id, pushJobId);

            return Ok(new StartPushingAccountsResponse
            {
                FetchJobId = fetchJobId,
                PushJobId = pushJobId,
                EstimatedNumberOfAccounts = estimatedCount
            });
        }

        /// <summary>
        /// POST
        /// /jobs/push/accounts/by/tags/ns/:ns
        /// +B, JOB
        /// Starts pushing list of all accounts tagged in a specific namespace to HTTP/Redis/Kafka target
        /// </summary>
        [HttpPost("jobs/push/accounts/by/tagns")]
        public async Task<IActionResult> StartPushingAccountsByTagNamespace(StartPushingAccountsByTagNsRequest input)
        {
            var validationResult = ValidateForStartPushingAccountsByTagNs(input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var query = new AccountQuery { TaggedInNs = input.TagNs };

            var estimatedCount = await _indexLogic.Count(query);
            if (estimatedCount <= 0)
                return Ok(new StartPushingAccountsResponse { EstimatedNumberOfAccounts = 0 });

            var pushJobId = await _jobLogic.CreatePushAccountsJob(
                input.Behavior, input.Target, (input.JobDisplayName ?? "job") + "_push", estimatedCount);
            var fetchJobId = await _jobLogic.CreateFetchAccountsJob(
                query, input.Behavior, (input.JobDisplayName ?? "job") + "_fetch", pushJobId, estimatedCount);

            await _jobManager.StartJob(_tenant.Id, fetchJobId);
            await _jobManager.StartJob(_tenant.Id, pushJobId);

            return Ok(new StartPushingAccountsResponse
            {
                FetchJobId = fetchJobId,
                PushJobId = pushJobId,
                EstimatedNumberOfAccounts = estimatedCount
            });
        }

        /// <summary>
        /// POST
        /// /jobs/push/accounts/by/tags/ns/:ns/t/:t
        /// +B, JOB
        /// Starts pushing list of all accounts tagged with a specific tag to HTTP/Redis/Kafka target
        /// </summary>
        [HttpPost("jobs/push/accounts/by/tag")]
        public async Task<IActionResult> StartPushingAccountsByTag(StartPushingAccountsByTagRequest input)
        {
            var validationResult = ValidateForStartPushingAccountsByTag(input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var query = new AccountQuery
            {
                TaggedWithAll = new List<FqTag> { new FqTag { Ns = input.TagNs, Tag = input.Tag } }
            };

            var estimatedCount = await _indexLogic.Count(query);
            if (estimatedCount <= 0)
                return Ok(new StartPushingAccountsResponse { EstimatedNumberOfAccounts = 0 });

            var pushJobId = await _jobLogic.CreatePushAccountsJob(
                input.Behavior, input.Target, (input.JobDisplayName ?? "job") + "_push", estimatedCount);
            var fetchJobId = await _jobLogic.CreateFetchAccountsJob(
                query, input.Behavior, (input.JobDisplayName ?? "job") + "_fetch", pushJobId, estimatedCount);

            await _jobManager.StartJob(_tenant.Id, fetchJobId);
            await _jobManager.StartJob(_tenant.Id, pushJobId);

            return Ok(new StartPushingAccountsResponse
            {
                FetchJobId = fetchJobId,
                PushJobId = pushJobId,
                EstimatedNumberOfAccounts = estimatedCount
            });
        }

        #region Validation methods

        private ApiValidationResult ValidateForStartPushingAccountsByTag(StartPushingAccountsByTagRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(ErrorKeys.InputCanNotBeEmpty);

            var result = ApiValidationResult.Aggregate(
                _validation.ValidateTagNs(input.TagNs),
                _validation.ValidateTag(input.Tag));

            if (input.JobDisplayName != null && input.JobDisplayName.Length > 50)
                result.Append(nameof(input.JobDisplayName), ErrorKeys.ArgumentLengthExceedsMaximum);

            result.Append(ValidatePushTarget(input.Target));
            result.Append(ValidatePushBehavior(input.Behavior));
            return result;
        }

        private ApiValidationResult ValidateForStartPushingAllAccounts(StartPushingAllAccountsRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(ErrorKeys.InputCanNotBeEmpty);

            var result = new ApiValidationResult();
            if (input.JobDisplayName != null && input.JobDisplayName.Length > 50)
                result.Append(nameof(input.JobDisplayName), ErrorKeys.ArgumentLengthExceedsMaximum);

            result.Append(ValidatePushTarget(input.Target));
            result.Append(ValidatePushBehavior(input.Behavior));
            return result;
        }

        private ApiValidationResult ValidateForStartPushingAccountsByQuery(StartPushingAccountsByQueryRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(ErrorKeys.InputCanNotBeEmpty);

            var result = _validation.ValidateAccountQuery(input.Query);
            if (input.JobDisplayName != null && input.JobDisplayName.Length > 50)
                result.Append(nameof(input.JobDisplayName), ErrorKeys.ArgumentLengthExceedsMaximum);

            result.Append(ValidatePushTarget(input.Target));
            result.Append(ValidatePushBehavior(input.Behavior));
            return result;
        }

        private ApiValidationResult ValidateForStartPushingAccountsByTagNs(StartPushingAccountsByTagNsRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(ErrorKeys.InputCanNotBeEmpty);

            var result = ApiValidationResult.Aggregate(
                _validation.ValidateTagNs(input.TagNs));

            if (input.JobDisplayName != null && input.JobDisplayName.Length > 50)
                result.Append(nameof(input.JobDisplayName), ErrorKeys.ArgumentLengthExceedsMaximum);

            result.Append(ValidatePushTarget(input.Target));
            result.Append(ValidatePushBehavior(input.Behavior));
            return result;
        }

        public ApiValidationResult ValidatePushTarget(PushTargetSpecification target)
        {
            var result = new ApiValidationResult();

            if (target == null)
                result.Append(nameof(target), ErrorKeys.ArgumentCanNotBeEmpty);

            if (target == null)
                return result;

            var numberOfTargets =
                (target.Http != null ? 1 : 0) +
                (target.Kafka != null ? 1 : 0) +
                (target.Redis != null ? 1 : 0);

            if (numberOfTargets < 1)
                result.Append(ErrorKeys.NoPushTargetSpecified);

            if (numberOfTargets > 1)
                result.Append(ErrorKeys.TooManyPushTargetsSpecified);

            if (target.Http != null)
                result.Append(ValidateHttpPushTarget(target.Http));

            if (target.Kafka != null)
                result.Append(ValidateKafkaPushTarget(target.Kafka));

            if (target.Redis != null)
                result.Append(ValidateRedisPushTarget(target.Redis));

            return result;
        }

        public ApiValidationResult ValidateHttpPushTarget(HttpPushTargetSpecification target)
        {
            var result = new ApiValidationResult();
            result.Append(_validation.ValidateTargetUrl(target.Url));

            if (target.MaxInstantRetries.HasValue)
                result.Append(_validation.ValidateRange(nameof(target.MaxInstantRetries), target.MaxInstantRetries.Value, 0, 5));

            if (target.MaxDelayedRetries.HasValue)
                result.Append(_validation.ValidateRange(nameof(target.MaxDelayedRetries), target.MaxDelayedRetries.Value, 0, 5));

            if (target.RetryDelaySeconds.HasValue)
                result.Append(_validation.ValidateRange(nameof(target.RetryDelaySeconds), target.RetryDelaySeconds.Value, 1, 2 * 60 * 60));

            if (target.HttpConcurrencyLevel.HasValue)
                result.Append(_validation.ValidateRange(nameof(target.HttpConcurrencyLevel), target.HttpConcurrencyLevel.Value, 1, 5000));

            return result;
        }

        public ApiValidationResult ValidateKafkaPushTarget(KafkaPushTargetSpecification target)
        {
            return new ApiValidationResult(new ApiValidationError("KafkaTarget", ErrorKeys.NotImplemented));
        }

        public ApiValidationResult ValidateRedisPushTarget(RedisPushTargetSpecification target)
        {
            return new ApiValidationResult(new ApiValidationError("RedisTarget", ErrorKeys.NotImplemented));
        }

        public ApiValidationResult ValidatePushBehavior(PushBehaviorSpecification behavior)
        {
            var result = new ApiValidationResult();

            if (behavior == null)
                return result;

            if (behavior.MaxBatchSize.HasValue)
                result.Append(_validation.ValidateRange(nameof(behavior.MaxBatchSize), behavior.MaxBatchSize.Value, 1, 500));

            if (behavior.AccountBatchesPerSecond < 0.01d)
                result.Append(new ApiValidationError(nameof(behavior.AccountBatchesPerSecond),
                    ErrorKeys.ArgumentIsSmallerThanMinimum));

            if (behavior.ExpireJobAfterSeconds.HasValue)
                result.Append(_validation.ValidateRange(nameof(behavior.ExpireJobAfterSeconds),
                    behavior.ExpireJobAfterSeconds.Value, 10, 24 * 60 * 60));

            if (behavior.TagWeightsToInclude != null)
            {
                if (behavior.TagWeightsToInclude.Count > 5)
                    result.Append(nameof(behavior.TagWeightsToInclude) + ".Count",
                        ErrorKeys.ArgumentIsLargerThanMaximum);

                behavior.TagWeightsToInclude.ForEach(t =>
                {
                    result.Append(_validation.ValidateTag(t.Tag));
                    result.Append(_validation.ValidateTagNs(t.Ns));
                });
            }

            if (behavior.FieldValuesToInclude != null)
            {
                if (behavior.FieldValuesToInclude.Count > 5)
                    result.Append(nameof(behavior.FieldValuesToInclude) + ".Count",
                        ErrorKeys.ArgumentIsLargerThanMaximum);

                behavior.FieldValuesToInclude.ForEach(f =>
                {
                    result.Append(_validation.ValidateFieldName(f));
                });
            }

            return result;
        }


        #endregion

    }
}
