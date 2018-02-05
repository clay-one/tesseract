namespace Tesseract.ApiModel.Jobs
{
    public class StartPushingAllAccountsRequest
    {
        public string JobDisplayName { get; set; }

        public PushBehaviorSpecification Behavior { get; set; }
        public PushTargetSpecification Target { get; set; }
    }
}