using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Job
{
    public interface IJobRunner
    {
        void Initialize(JobData jobData);
        Task<bool> CheckHealth();
        void StopRunner();

        string JobId { get; }
        bool IsProcessRunning { get; }
        bool IsProcessTerminated { get; }
    }
    
    [Contract]
    public interface IJobRunner<TJobStep> : IJobRunner where TJobStep : JobStepBase
    {
    }
}