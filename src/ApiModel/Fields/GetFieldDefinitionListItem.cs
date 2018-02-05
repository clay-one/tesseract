using System;

namespace Tesseract.ApiModel.Fields
{
    public class GetFieldDefinitionListItem
    {
        public string FieldName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }

        public bool KeepHistory { get; set; }
    }
}