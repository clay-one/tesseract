using System.Threading.Tasks;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IStaticJob
    {
        Task EnsureJobsDefined();
    }
}