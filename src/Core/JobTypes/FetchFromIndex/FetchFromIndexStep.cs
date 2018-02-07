using Tesseract.Core.Queue;

namespace Tesseract.Core.JobTypes.FetchFromIndex
{
    public class FetchFromIndexStep : JobStepBase
    {
        public int SliceId { get; set; }
        public int Sequence { get; set; }
        public string ScrollId { get; set; }
    }
}