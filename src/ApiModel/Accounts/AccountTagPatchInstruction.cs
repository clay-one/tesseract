namespace Tesseract.ApiModel.Accounts
{
    public class AccountTagPatchInstruction
    {
        public string TagNs { get; set; }
        public string Tag { get; set; }
        public double WeightDelta { get; set; }
    }
}