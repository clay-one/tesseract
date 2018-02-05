namespace Tesseract.ApiModel.Accounts
{
    public class PatchTagWeightOnAccountResponse
    {
        public string AccountId { get; set; }
        public string TagNs { get; set; }
        public string Tag { get; set; }
        public double Weight { get; set; }
    }
}