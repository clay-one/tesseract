using Tesseract.ApiModel.General;

namespace Tesseract.ApiModel.Accounts
{
    public class GetAccountQueryResultsRequest
    {
        public AccountQuery Query { get; set; }

        public int? Count { get; set; }
        public string ContinueFrom { get; set; }
    }
}