namespace Tesseract.ApiModel.Accounts
{
    public class GetFieldValueOnAccountResponse
    {
        public string AccountId { get; set; }
        public string FieldName { get; set; }
        public double FieldValue { get; set; }
    }
}