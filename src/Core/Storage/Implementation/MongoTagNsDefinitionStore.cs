using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Tesseract.ApiModel.Tags;
using Tesseract.Core.Connection;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Storage.Implementation
{
    public class MongoTagNsDefinitionStore : ITagNsDefinitionStore
    {
        private readonly IMongoManager _mongo;

        public MongoTagNsDefinitionStore(IMongoManager mongo)
        {
            _mongo = mongo;
        }

        public async Task<List<TagNsDefinitionData>> LoadAll(string tenantId)
        {
            var filter = Builders<TagNsDefinitionData>.Filter.Eq(nsdd => nsdd.TenantId, tenantId);

            var cursor = await _mongo.TagNsDefinitions.FindAsync(filter);
            return await cursor.ToListAsync();
        }

        public async Task<TagNsDefinitionData> Load(string tenantId, string ns)
        {
            var filter = Builders<TagNsDefinitionData>.Filter.And(
                Builders<TagNsDefinitionData>.Filter.Eq(nsdd => nsdd.TenantId, tenantId),
                Builders<TagNsDefinitionData>.Filter.Eq(nsdd => nsdd.Namespace, ns)
            );

            var cursor = await _mongo.TagNsDefinitions.FindAsync(filter);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<TagNsDefinitionData> AddOrUpdate(string tenantId, string ns, PutTagNsDefinitionRequest data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var filter = Builders<TagNsDefinitionData>.Filter.And(
                Builders<TagNsDefinitionData>.Filter.Eq(nsdd => nsdd.TenantId, tenantId),
                Builders<TagNsDefinitionData>.Filter.Eq(nsdd => nsdd.Namespace, ns)
            );

            var update = Builders<TagNsDefinitionData>.Update
                .SetOnInsert(nsdd => nsdd.TenantId, tenantId)
                .SetOnInsert(nsdd => nsdd.Namespace, ns)
                .SetOnInsert(nsdd => nsdd.CreationTime, DateTime.UtcNow)
                .SetOnInsert(nsdd => nsdd.CreatedBy, "unknown")
                .Set(nsdd => nsdd.LastModificationTime, DateTime.UtcNow)
                .Set(nsdd => nsdd.LastModifiedBy, "unknown")
                .Set(nsdd => nsdd.KeepHistory, data.KeepHistory);

            return await _mongo.TagNsDefinitions.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<TagNsDefinitionData>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                });
        }

        public async Task<TagNsDefinitionData> Remove(string tenantId, string ns)
        {
            var filter = Builders<TagNsDefinitionData>.Filter.And(
                Builders<TagNsDefinitionData>.Filter.Eq(nsdd => nsdd.TenantId, tenantId),
                Builders<TagNsDefinitionData>.Filter.Eq(nsdd => nsdd.Namespace, ns)
            );

            return await _mongo.TagNsDefinitions.FindOneAndDeleteAsync(filter);
        }
    }
}