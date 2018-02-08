using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;
using Tesseract.Common.Results;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IJobManager
    {
        Task CleanupOldJobs();

        Task<string> CreateNewJobOrUpdateDefinition<TJobStep>(string tenantId,
            string jobDisplayName = null, string jobId = null, JobConfigurationData configuration = null)
            where TJobStep : JobStepBase;

        Task AddPredecessor(string tenantId, string jobId, string predecessorJobId);

        Task StartJob(string tenantId, string jobId);
        Task StartJobIfNotStarted(string tenantId, string jobId);
        Task<ApiValidationResult> StopJob(string tenantId, string jobId);
        Task<ApiValidationResult> PauseJob(string tenantId, string jobId);
        Task<ApiValidationResult> DrainJob(string tenantId, string jobId);
        Task<ApiValidationResult> ResumeJob(string tenantId, string jobId);
        Task<ApiValidationResult> PurgeJobQueue(string tenantId, string jobId);
        Task<long> GetQueueLength(string tenantId, string jobId);
    }
}