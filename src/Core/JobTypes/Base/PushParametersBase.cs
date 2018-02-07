using System.Collections.Generic;

namespace Tesseract.Core.JobTypes.Base
{
    public abstract class PushParametersBase
    {
        public List<FqTag> TagWeightsToInclude { get; set; }
        public List<string> FieldValuesToInclude { get; set; }
    }
}