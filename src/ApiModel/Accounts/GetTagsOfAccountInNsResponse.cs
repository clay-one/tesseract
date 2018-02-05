using System.Collections.Generic;

namespace Tesseract.ApiModel.Accounts
{
    public class GetTagsOfAccountInNsResponse
    {
        public string AccountId { get; set; }
        public string Namespace { get; set; }
        public List<TagWeightOnAccount> Tags { get; set; }
    }
}