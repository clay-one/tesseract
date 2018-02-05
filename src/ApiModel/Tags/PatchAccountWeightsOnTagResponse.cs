using System.Collections.Generic;

namespace Tesseract.ApiModel.Tags
{
    public class PatchAccountWeightsOnTagResponse
    {
        public string TagNs { get; set; }
        public string Tag { get; set; }

        public List<AccountWeightOnTag> Accounts { get; set; }
    }
}