using System.Threading.Tasks;
using ComposerCore.Attributes;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IJobNotification
    {
        Task StartNotificationTargetThread();
        Task StopNotificationTargetThread();

        Task NotifyJobUpdated(string jobId);
    }
}