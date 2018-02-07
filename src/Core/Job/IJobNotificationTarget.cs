using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IJobNotificationTarget
    {
        Task ProcessNotification(string jobId);
    }
}