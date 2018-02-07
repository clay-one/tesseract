using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;

namespace Tesseract.Core.Job.Implementation
{
    [Component]
    public class DefaultJobNotificationTarget : IJobNotificationTarget
    {
        [ComponentPlug]
        public IJobRunnerManager RunnerManager { get; set; }

        public async Task ProcessNotification(string jobId)
        {
            await RunnerManager.CheckHealthOrCreateRunner(jobId);
        }
    }
}