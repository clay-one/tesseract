using System;
using System.Threading.Tasks;
using ComposerCore.Attributes;

namespace Tesseract.Core.Distribution
{
    [Contract]
    public interface ICommonTimestamp
    {
        Task Initialize();
        DateTime GetTime();
    }
}