using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComposerCore.Attributes;
using MongoDB.Driver;
using Tesseract.ApiModel.Fields;
using Tesseract.Core.Connection;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Storage.Implementation
{
    [Component]
    public class MongoFieldDefinitionStore : IFieldDefinitionStore
    {
        [ComponentPlug]
        public IMongoManager Mongo { get; set; }

        public async Task<List<FieldDefinitionData>> LoadAll(string tenantId)
        {
            var filter = Builders<FieldDefinitionData>.Filter.Eq(nsdd => nsdd.TenantId, tenantId);

            var cursor = await Mongo.FieldDefinitions.FindAsync(filter);
            return await cursor.ToListAsync();
        }

        public async Task<FieldDefinitionData> Load(string tenantId, string fieldName)
        {
            var filter = Builders<FieldDefinitionData>.Filter.And(
                Builders<FieldDefinitionData>.Filter.Eq(nsdd => nsdd.TenantId, tenantId),
                Builders<FieldDefinitionData>.Filter.Eq(nsdd => nsdd.FieldName, fieldName)
            );

            var cursor = await Mongo.FieldDefinitions.FindAsync(filter);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<FieldDefinitionData> AddOrUpdate(string tenantId, string fieldName,
            PutFieldDefinitionRequest data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var filter = Builders<FieldDefinitionData>.Filter.And(
                Builders<FieldDefinitionData>.Filter.Eq(fdd => fdd.TenantId, tenantId),
                Builders<FieldDefinitionData>.Filter.Eq(fdd => fdd.FieldName, fieldName)
            );

            var update = Builders<FieldDefinitionData>.Update
                .SetOnInsert(fdd => fdd.TenantId, tenantId)
                .SetOnInsert(fdd => fdd.FieldName, fieldName)
                .SetOnInsert(fdd => fdd.CreationTime, DateTime.UtcNow)
                .SetOnInsert(fdd => fdd.CreatedBy, "unknown")
                .Set(fdd => fdd.LastModificationTime, DateTime.UtcNow)
                .Set(fdd => fdd.LastModifiedBy, "unknown")
                .Set(fdd => fdd.KeepHistory, data.KeepHistory);

            return await Mongo.FieldDefinitions.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<FieldDefinitionData>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                });
        }

        public async Task<FieldDefinitionData> Remove(string tenantId, string fieldName)
        {
            var filter = Builders<FieldDefinitionData>.Filter.And(
                Builders<FieldDefinitionData>.Filter.Eq(nsdd => nsdd.TenantId, tenantId),
                Builders<FieldDefinitionData>.Filter.Eq(nsdd => nsdd.FieldName, fieldName)
            );

            return await Mongo.FieldDefinitions.FindOneAndDeleteAsync(filter);
        }
    }
}