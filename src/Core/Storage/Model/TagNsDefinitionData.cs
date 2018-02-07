using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tesseract.Core.Storage.Model
{
    public class TagNsDefinitionData
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string TenantId { get; set; }
        public string Namespace { get; set; }

        public DateTime CreationTime { get; set; }
        public string CreatedBy { get; set; }

        public DateTime LastModificationTime { get; set; }
        public string LastModifiedBy { get; set; }

        public bool KeepHistory { get; set; }

    }
}