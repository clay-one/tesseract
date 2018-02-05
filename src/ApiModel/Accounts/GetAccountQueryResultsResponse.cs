using System.Collections.Generic;

namespace Tesseract.ApiModel.Accounts
{
    public class GetAccountQueryResultsResponse
    {
        public long RequestedCount { get; set; }
        public string RequestedContinueFrom { get; set; }

        public List<GetAccountQueryResultsResponseItem> Accounts { get; set; }
        public long TotalNumberOfResults { get; set; }

        public string ContinueWith { get; set; }
    }
}