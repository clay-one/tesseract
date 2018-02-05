namespace Tesseract.ApiModel.Jobs
{
    public class PushTargetSpecification
    {
        public HttpPushTargetSpecification Http { get; set; }
        public KafkaPushTargetSpecification Kafka { get; set; }
        public RedisPushTargetSpecification Redis { get; set; }
    }
}