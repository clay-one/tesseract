using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Tesseract.Core.Storage.Model
{
    public class AccountData
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string TenantId { get; set; }
        public string AccountId { get; set; }

        public DateTime CreationTime { get; set; }
        public string CreatedBy { get; set; }

        public DateTime LastModificationTime { get; set; }
        public string LastModifiedBy { get; set; }

        public DateTime? LastReindexTime { get; set; }
        public bool HasFreshIndex { get; set; }


        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, Dictionary<string, double>> TagNamespaces { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, double> Fields { get; set; }

    }
}