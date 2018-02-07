using Tesseract.Core.JobTypes.Base;

namespace Tesseract.Core.JobTypes.AccountIndexing
{
    public class AccountIndexingStep : IndexingStepBase
    {
        public string TenantId { get; set; }
        public string AccountId { get; set; }
    }
}