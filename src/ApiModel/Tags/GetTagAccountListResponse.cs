using System.Collections.Generic;

namespace Tesseract.ApiModel.Tags
{
    public class GetTagAccountListResponse
    {
        public string RequestedTagNs { get; set; }
        public string RequestedTag { get; set; }
        public long RequestedCount { get; set; }
        public string RequestedContinueFrom { get; set; }

        public List<GetTagAccountListResponseItem> Accounts { get; set; }
        public long TotalNumberOfResults { get; set; }

        public string ContinueWith { get; set; }
    }
}