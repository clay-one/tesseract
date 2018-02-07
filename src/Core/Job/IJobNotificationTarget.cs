using System.Threading.Tasks;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IJobNotificationTarget
    {
        Task ProcessNotification(string jobId);
    }
}