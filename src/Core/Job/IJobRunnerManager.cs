using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IJobRunnerManager
    {
        Task CheckStoreJobs();
        Task CheckHealthOfAllRunners();
        
        Task CheckHealthOrCreateRunner(string jobId);
        void StopAllRunners();
        
        bool IsJobRunnerActive(string jobId);
    }
}