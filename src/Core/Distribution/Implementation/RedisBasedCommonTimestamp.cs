using System;
using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;

namespace Tesseract.Core.Distribution.Implementation
{
    [Component]
    public class RedisBasedCommonTimestamp : ICommonTimestamp
    {
        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public DateTime GetTime()
        {
            return DateTime.UtcNow;
        }
    }
}