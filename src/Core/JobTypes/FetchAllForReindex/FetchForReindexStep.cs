using Tesseract.Core.Queue;

namespace Tesseract.Core.JobTypes.FetchAllForReindex
{
    public class FetchForReindexStep : JobStepBase
    {
        public string TenantId { get; set; }

        public string RangeStart { get; set; }
        public string RangeEnd { get; set; }
        public string LastAccountId { get; set; }
    }
}