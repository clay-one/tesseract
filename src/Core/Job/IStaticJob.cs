using System.Threading.Tasks;
using ComposerCore.Attributes;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IStaticJob
    {
        Task EnsureJobsDefined();
    }
}