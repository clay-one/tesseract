namespace Tesseract.ApiModel.General
{
    public class AccountFieldQuery
    {
        public string FieldName { get; set; }
        public double? LowerBound { get; set; }
        public double? UpperBound { get; set; }
    }
}