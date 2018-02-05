using System.Collections.Generic;

namespace Tesseract.ApiModel.General
{
    public class AccountQuery
    {
        public List<AccountQuery> And { get; set; }
        public List<AccountQuery> Or { get; set; }
        public AccountQuery Not { get; set; }

        public List<FqTag> TaggedWithAll { get; set; }
        public List<FqTag> TaggedWithAny { get; set; }
        public string TaggedInNs { get; set; }
        public AccountFieldQuery FieldWithin { get; set; }
    }
}