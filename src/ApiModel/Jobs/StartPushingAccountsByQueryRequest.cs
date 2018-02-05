using Tesseract.ApiModel.General;

namespace Tesseract.ApiModel.Jobs
{
    public class StartPushingAccountsByQueryRequest
    {
        public AccountQuery Query { get; set; }

        public string JobDisplayName { get; set; }

        public PushBehaviorSpecification Behavior { get; set; }
        public PushTargetSpecification Target { get; set; }
    }
}