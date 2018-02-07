using Tesseract.ApiModel.General;

namespace Tesseract.Core.JobTypes.FetchFromIndex
{
    public class FetchFromIndexParameters
    {
        public string TenantId { get; set; }
        public string TargetJobId { get; set; }
        public AccountQuery Query { get; set; }
        public int SliceCount { get; set; }
        public int MaxBatchSize { get; set; }
    }
}