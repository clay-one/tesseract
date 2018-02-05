namespace Tesseract.ApiModel.Jobs
{
    public class StartPushingAccountsByTagRequest
    {
        public string TagNs { get; set; }
        public string Tag { get; set; }

        public string JobDisplayName { get; set; }

        public PushBehaviorSpecification Behavior { get; set; }
        public PushTargetSpecification Target { get; set; }
    }
}