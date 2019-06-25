using System.Threading.Tasks;

namespace Tesseract.Core.Job
{
    public interface IJobRunnerManager
    {
        Task CheckStoreJobs();
        Task CheckHealthOfAllRunners();

        Task CheckHealthOrCreateRunner(string jobId);
        void StopAllRunners();

        bool IsJobRunnerActive(string jobId);
    }
}