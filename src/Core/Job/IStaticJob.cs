using System.Threading.Tasks;

namespace Tesseract.Core.Job
{
    public interface IStaticJob
    {
        Task EnsureJobsDefined();
    }
}