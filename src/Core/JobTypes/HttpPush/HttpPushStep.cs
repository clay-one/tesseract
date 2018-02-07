using System;
using Tesseract.Core.JobTypes.Base;

namespace Tesseract.Core.JobTypes.HttpPush
{
    public class HttpPushStep : PushStepBase
    {
        public int? NumberOfAttempts { get; set; }
        public DateTime? LastAttemptTime { get; set; }
    }
}