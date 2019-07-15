using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;
using Tesseract.Common.Results;
using Tesseract.Core;
using Tesseract.Core.Job;
using Tesseract.Core.Logic;

namespace Appson.Tesseract.Web.Controllers
{
    public class JobActionsController : ApiControllerBase
    {
        // PUT	/jobs/j/:j/actions/resume	-B, IDMP	Changes a job's status back to "InProgress" to continue working normally, if the job is paused or draining.
        // PUT	/jobs/j/:j/actions/pause	-B, IDMP	Changes a job's status to "Paused", and stops workers of the job temporarily until resumed.
        // PUT	/jobs/j/:j/actions/drain	-B, IDMP	Changes a job's status to "Draining", and causes the job queue items to be drained without processing.
        // PUT	/jobs/j/:j/actions/stop	-B, IDMP	Stops a job and all its preprocessors permanently, and deletes all of the job queue items.
        // PUT	/jobs/j/:j/actions/purge-queue	-B, IDMP	Purges the current job queue without any change in the job status.


        private readonly ICurrentTenantLogic _tenant;
        private readonly IJobManager _jobManager;

        public JobActionsController(ICurrentTenantLogic currentTenantLogic, IJobManager jobManager)
        {
            _jobManager = jobManager;
            _tenant = currentTenantLogic;
        }

        [HttpPut("jobs/j/{jobId}/actions/resume")]
        public async Task<IActionResult> ResumeJob(string jobId)
        {
            var validationResult = ValidateForResumeJob(jobId);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            return ValidationResult(await _jobManager.ResumeJob(_tenant.Id, jobId));
        }

        [HttpPut("jobs/j/{jobId}/actions/pause")]
        public async Task<IActionResult> PauseJob(string jobId)
        {
            var validationResult = ValidateForPauseJob(jobId);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            return ValidationResult(await _jobManager.PauseJob(_tenant.Id, jobId));
        }

        [HttpPut("jobs/j/{jobId}/actions/drain")]
        public async Task<IActionResult> DrainJob(string jobId)
        {
            var validationResult = ValidateForDrainJob(jobId);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            return ValidationResult(await _jobManager.DrainJob(_tenant.Id, jobId));
        }

        [HttpPut("jobs/j/{jobId}/actions/stop")]
        public async Task<IActionResult> StopJob(string jobId)
        {
            var validationResult = ValidateForStopJob(jobId);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            return ValidationResult(await _jobManager.StopJob(_tenant.Id, jobId));
        }

        [HttpPut("jobs/j/{jobId}/actions/purge-queue")]
        public async Task<IActionResult> PurgeJobQueue(string jobId)
        {
            var validationResult = ValidateForPurgeJobQueue(jobId);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            return ValidationResult(await _jobManager.PurgeJobQueue(_tenant.Id, jobId));
        }

        #region Validation methods

        private ApiValidationResult ValidateForResumeJob(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                return ApiValidationResult.Failure(nameof(jobId), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            return ApiValidationResult.Ok();
        }

        private ApiValidationResult ValidateForPauseJob(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                return ApiValidationResult.Failure(nameof(jobId), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            return ApiValidationResult.Ok();
        }

        private ApiValidationResult ValidateForDrainJob(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                return ApiValidationResult.Failure(nameof(jobId), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            return ApiValidationResult.Ok();
        }

        private ApiValidationResult ValidateForStopJob(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                return ApiValidationResult.Failure(nameof(jobId), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            return ApiValidationResult.Ok();
        }

        private ApiValidationResult ValidateForPurgeJobQueue(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                return ApiValidationResult.Failure(nameof(jobId), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            return ApiValidationResult.Ok();
        }

        #endregion

    }
}