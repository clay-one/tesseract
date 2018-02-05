namespace Tesseract.ApiModel.Jobs
{
    public class StartPushingAccountsByTagNsRequest
    {
        public string TagNs { get; set; }

        public string JobDisplayName { get; set; }

        public PushBehaviorSpecification Behavior { get; set; }
        public PushTargetSpecification Target { get; set; }
    }
}