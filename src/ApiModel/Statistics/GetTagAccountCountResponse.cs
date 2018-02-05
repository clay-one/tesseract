namespace Tesseract.ApiModel.Statistics
{
    public class GetTagAccountCountResponse
    {
        public string TagNs { get; set; }
        public string Tag { get; set; }
        public long TotalCount { get; set; }
    }
}