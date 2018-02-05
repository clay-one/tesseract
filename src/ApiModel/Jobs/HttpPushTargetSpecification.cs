namespace Tesseract.ApiModel.Jobs
{
    public class HttpPushTargetSpecification
    {
        public string Url { get; set; }

        public bool IgnoreHttpErrors { get; set; }

        public int? MaxInstantRetries { get; set; }
        public int? MaxDelayedRetries { get; set; }
        public int? RetryDelaySeconds { get; set; }

        public int? HttpConcurrencyLevel { get; set; }
    }
}