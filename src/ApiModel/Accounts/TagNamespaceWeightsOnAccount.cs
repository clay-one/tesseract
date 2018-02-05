using System.Collections.Generic;

namespace Tesseract.ApiModel.Accounts
{
    public class TagNamespaceWeightsOnAccount
    {
        public string Namespace { get; set; }
        public List<TagWeightOnAccount> Tags { get; set; }
    }
}