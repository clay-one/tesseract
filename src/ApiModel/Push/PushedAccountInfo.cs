using System.Collections.Generic;
using Tesseract.ApiModel.General;

namespace Tesseract.ApiModel.Push
{
    public class PushedAccountInfo
    {
        public string AccountId { get; set; }
        public List<FqTagWithWeight> TagWeights { get; set; }
        public List<FieldNameWithValue> FieldValues { get; set; }
    }
}