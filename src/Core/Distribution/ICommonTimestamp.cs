using System;
using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;

namespace Tesseract.Core.Distribution
{
    [Contract]
    public interface ICommonTimestamp
    {
        Task Initialize();
        DateTime GetTime();
    }
}