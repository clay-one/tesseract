using System.Collections.Generic;

namespace Tesseract.ApiModel.Accounts
{
    public class GetAllTagsOfAccountResponse
    {
        public string AccountId { get; set; }

        public List<TagNamespaceWeightsOnAccount> Namespaces { get; set; }
    }
}