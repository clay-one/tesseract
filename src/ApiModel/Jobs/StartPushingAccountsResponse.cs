namespace Tesseract.ApiModel.Jobs
{
    public class StartPushingAccountsResponse
    {
        public string PushJobId { get; set; }
        public string FetchJobId { get; set; }

        public long EstimatedNumberOfAccounts { get; set; }
    }
}