namespace Tesseract.ApiModel.Accounts
{
    public class AccountFieldPatchInstruction
    {
        public string FieldName { get; set; }
        public double FieldValueDelta { get; set; }
    }
}