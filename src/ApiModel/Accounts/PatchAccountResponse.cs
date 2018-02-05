using System.Collections.Generic;

namespace Tesseract.ApiModel.Accounts
{
    public class PatchAccountResponse
    {
        public string AccountId { get; set; }

        public List<TagNamespaceWeightsOnAccount> TagNamespaces { get; set; }
        public List<FieldValueOnAccount> Fields { get; set; }
    }
}