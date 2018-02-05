namespace Tesseract.ApiModel.Accounts
{
    public class AccountTagChangeInstruction
    {
        public string TagNs { get; set; }
        public string Tag { get; set; }
        public double Weight { get; set; }
    }
}