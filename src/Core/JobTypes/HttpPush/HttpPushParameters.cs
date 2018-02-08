using Tesseract.Core.JobTypes.Base;

namespace Tesseract.Core.JobTypes.HttpPush
{
    public class HttpPushParameters : PushParametersBase
    {
        public string Url { get; set; }

        public bool IgnoreHttpErrors { get; set; }

        public int MaxInstantRetries { get; set; }
        public int MaxDelayedRetries { get; set; }
        public int RetryDelaySeconds { get; set; }
    }
}