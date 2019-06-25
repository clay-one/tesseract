using System.Threading.Tasks;

namespace Tesseract.Core.Job
{
    public interface IJobNotificationTarget
    {
        Task ProcessNotification(string jobId);
    }
}