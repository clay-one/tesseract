using System.Collections.Generic;
using Tesseract.Core.Queue;

namespace Tesseract.Core.JobTypes.Base
{
    public class PushStepBase : JobStepBase
    {
        public List<string> AccountIds { get; set; }
    }
}