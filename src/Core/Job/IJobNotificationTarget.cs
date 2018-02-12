using System.Threading.Tasks;
using ComposerCore.Attributes;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IJobNotificationTarget
    {
        Task ProcessNotification(string jobId);
    }
}