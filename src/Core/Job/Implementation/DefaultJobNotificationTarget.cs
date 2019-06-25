using System.Threading.Tasks;

namespace Tesseract.Core.Job.Implementation
{
    public class DefaultJobNotificationTarget : IJobNotificationTarget
    {
        private readonly IJobRunnerManager _runnerManager;

        public DefaultJobNotificationTarget(IJobRunnerManager jobRunnerManager)
        {
            _runnerManager = jobRunnerManager;
        }

        public async Task ProcessNotification(string jobId)
        {
            await _runnerManager.CheckHealthOrCreateRunner(jobId);
        }
    }
}