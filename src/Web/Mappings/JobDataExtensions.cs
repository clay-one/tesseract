using System.Linq;
using Hydrogen.General.Collections;
using Tesseract.ApiModel.Jobs;
using Tesseract.Core.Storage.Model;

namespace Appson.Tesseract.Web.Mappings
{
    public static class JobDataExtensions
    {
        public static GetJobStatusResponse ToGetJobStatusResponse(this JobData jobData)
        {
            return new GetJobStatusResponse
            {
                JobId = jobData.JobId,
                JobDisplayName = jobData.JobDisplayName,
                State = jobData.Status.State.ToString(),
                StateTime = jobData.Status.StateTime,
                IsCompleted = jobData.Status.State >= JobState.Completed,
                LastActivityTime = jobData.Status.LastIterationStartTime,
                LastProcessTime = jobData.Status.LastProcessFinishTime,
                LastHealthCheckTime = jobData.Status.LastHealthCheckTime,
                ItemsProcessed = jobData.Status.ItemsProcessed,
                ItemsFailed = jobData.Status.ItemsFailed,
                ItemsRequeued = jobData.Status.ItemsRequeued,
                ItemsGeneratedForTargetQueue = jobData.Status.ItemsGeneratedForTargetQueue,
                EstimatedTotalItems = jobData.Status.EstimatedTotalItems,
                ProcessingTimeTakenMillis = jobData.Status.ProcessingTimeTakenMillis,
                ExceptionCount = jobData.Status.ExceptionCount,
                LastExceptionTime = jobData.Status.LastExceptionTime,
                LastFailTime = jobData.Status.LastFailTime,
                LastFailures = jobData.Status.LastFailures.SafeSelect(f => f.ErrorMessage).ToArray(),
                CreationTime = jobData.CreationTime,
                PreprocessorJobIds = jobData.Configuration.PreprocessorJobIds,
            };
        }

        public static GetJobListResponseItem ToGetJobListResponseItem(this JobData jobData)
        {
            return new GetJobListResponseItem
            {
                JobId = jobData.JobId,
                State = jobData.Status.State.ToString(),
                StateTime = jobData.Status.StateTime,
                IsCompleted = jobData.Status.State >= JobState.Completed,
                JobDisplayName = jobData.JobDisplayName,
                CreationTime = jobData.CreationTime
            };
        }
    }
}