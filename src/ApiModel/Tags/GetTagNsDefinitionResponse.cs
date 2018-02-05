using System;

namespace Tesseract.ApiModel.Tags
{
    public class GetTagNsDefinitionResponse
    {
        public string TagNamespace { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }

        public bool KeepHistory { get; set; }
    }
}