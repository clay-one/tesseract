using System;
using System.Threading.Tasks;

namespace Tesseract.Core.Distribution
{
    [Contract]
    public interface ICommonTimestamp
    {
        Task Initialize();
        DateTime GetTime();
    }
}