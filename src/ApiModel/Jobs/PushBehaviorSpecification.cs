using System.Collections.Generic;
using Tesseract.ApiModel.General;

namespace Tesseract.ApiModel.Jobs
{
    public class PushBehaviorSpecification
    {
        public int? MaxBatchSize { get; set; }
        public double? AccountBatchesPerSecond { get; set; }
        public int? ExpireJobAfterSeconds { get; set; }

        public List<FqTag> TagWeightsToInclude { get; set; }
        public List<string> FieldValuesToInclude { get; set; }
    }
}