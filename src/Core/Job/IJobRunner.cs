using System.Threading.Tasks;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Job
{
    public interface IJobRunner
    {
        string JobId { get; }
        bool IsProcessRunning { get; }
        bool IsProcessTerminated { get; }
        void Initialize(JobData jobData);
        Task<bool> CheckHealth();
        void StopRunner();
    }

    public interface IJobRunner<TJobStep> : IJobRunner where TJobStep : JobStepBase
    {
    }
}