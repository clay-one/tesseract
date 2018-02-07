using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.Core.Connection;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Storage.Implementation
{
    [Component]
    public class MongoAccountStore : IAccountStore
    {
        [ComponentPlug]
        public IMongoManager Mongo { get; set; }

        public async Task<AccountData> LoadAccount(string tenantId, string accountId)
        {
            var filter = Builders<AccountData>.Filter.And(
                Builders<AccountData>.Filter.Eq(ai => ai.TenantId, tenantId),
                Builders<AccountData>.Filter.Eq(ai => ai.AccountId, accountId)
            );

            var cursor = await Mongo.Accounts.FindAsync(filter);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<List<AccountData>> LoadAccounts(string tenantId, IEnumerable<string> accountIds)
        {
            var filter = Builders<AccountData>.Filter.And(
                Builders<AccountData>.Filter.Eq(ai => ai.TenantId, tenantId),
                Builders<AccountData>.Filter.In(ai => ai.AccountId, accountIds)
            );

            var cursor = await Mongo.Accounts.FindAsync(filter);
            return await cursor.ToListAsync();
        }

        public async Task<AccountData> ChangeAccount(string tenantId, string accountId, PatchAccountRequest patch)
        {
            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            var filter = Builders<AccountData>.Filter.And(
                Builders<AccountData>.Filter.Eq(ai => ai.TenantId, tenantId),
                Builders<AccountData>.Filter.Eq(ai => ai.AccountId, accountId)
            );

            var update = Builders<AccountData>.Update
                .SetOnInsert(ai => ai.TenantId, tenantId)
                .SetOnInsert(ai => ai.AccountId, accountId)
                .SetOnInsert(ai => ai.CreationTime, DateTime.UtcNow)
                .SetOnInsert(ai => ai.CreatedBy, "unknown")
                .Set(ai => ai.LastModificationTime, DateTime.UtcNow)
                .Set(ai => ai.LastModifiedBy, "unknown")
                .Set(ai => ai.HasFreshIndex, false);

            patch.TagChanges?.ForEach(tc =>
            {
                update = update.Set(ai => ai.TagNamespaces[tc.TagNs][tc.Tag], tc.Weight);
            });

            patch.TagPatches?.ForEach(tp =>
            {
                update = update.Inc(ai => ai.TagNamespaces[tp.TagNs][tp.Tag], tp.WeightDelta);
            });

            patch.FieldChanges?.ForEach(fc =>
            {
                update = update.Set(ai => ai.Fields[fc.FieldName], fc.FieldValue);
            });

            patch.FieldPatches?.ForEach(fp =>
            {
                update = update.Inc(ai => ai.Fields[fp.FieldName], fp.FieldValueDelta);
            });

            return await Mongo.Accounts.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<AccountData> { IsUpsert = true, ReturnDocument = ReturnDocument.After });
        }

        public async Task<AccountData> SetTagWeightIfTagDoesntExist(string tenantId, string accountId, string ns, string tag, double weight)
        {
            var filter = Builders<AccountData>.Filter.And(
                Builders<AccountData>.Filter.Eq(ai => ai.TenantId, tenantId),
                Builders<AccountData>.Filter.Eq(ai => ai.AccountId, accountId),
                Builders<AccountData>.Filter.Not(Builders<AccountData>.Filter.Exists(ai => ai.TagNamespaces[ns][tag])));

            var update = Builders<AccountData>.Update
                .Set(ai => ai.LastModifiedBy, "unknown")
                .Set(ai => ai.LastModificationTime, DateTime.UtcNow)
                .Set(ai => ai.HasFreshIndex, false)
                .Set(ai => ai.TagNamespaces[ns][tag], weight);

            return await Mongo.Accounts.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<AccountData> { IsUpsert = false, ReturnDocument = ReturnDocument.After });
        }

        public async Task<AccountData> SetTagWeightIfAccountDoesntExist(string tenantId, string accountId, string ns, string tag, double weight)
        {
            var filter = Builders<AccountData>.Filter.And(
                Builders<AccountData>.Filter.Eq(ai => ai.TenantId, tenantId),
                Builders<AccountData>.Filter.Eq(ai => ai.AccountId, accountId)
            );

            var update = Builders<AccountData>.Update
                .SetOnInsert(ai => ai.TenantId, tenantId)
                .SetOnInsert(ai => ai.AccountId, accountId)
                .SetOnInsert(ai => ai.CreationTime, DateTime.UtcNow)
                .SetOnInsert(ai => ai.LastModifiedBy, "unknown")
                .SetOnInsert(ai => ai.LastModificationTime, DateTime.UtcNow)
                .SetOnInsert(ai => ai.HasFreshIndex, false)
                .SetOnInsert(ai => ai.TagNamespaces[ns][tag], weight);

            return await Mongo.Accounts.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<AccountData> { IsUpsert = true, ReturnDocument = ReturnDocument.After });
        }

        public async Task<AccountData> RemoveTags(string tenantId, string accountId, IEnumerable<FqTag> fqTags)
        {
            if (fqTags == null)
                throw new ArgumentNullException(nameof(fqTags));

            var filter = Builders<AccountData>.Filter.And(
                Builders<AccountData>.Filter.Eq(ai => ai.TenantId, tenantId),
                Builders<AccountData>.Filter.Eq(ai => ai.AccountId, accountId)
            );

            var update = Builders<AccountData>.Update
                .Set(ai => ai.LastModifiedBy, "unknown")
                .Set(ai => ai.LastModificationTime, DateTime.UtcNow)
                .Set(ai => ai.HasFreshIndex, false);

            fqTags.ForEach(t =>
            {
                update = update.Unset(ai => ai.TagNamespaces[t.Ns][t.Tag]);
            });

            var result = await Mongo.Accounts.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<AccountData> { IsUpsert = false, ReturnDocument = ReturnDocument.After });

            return result;
        }

        public async Task<AccountData> RemoveTagNs(string tenantId, string accountId, string ns)
        {
            var filter = Builders<AccountData>.Filter.And(
                Builders<AccountData>.Filter.Eq(ai => ai.TenantId, tenantId),
                Builders<AccountData>.Filter.Eq(ai => ai.AccountId, accountId)
            );

            var update = Builders<AccountData>.Update
                .Set(ai => ai.LastModifiedBy, "unknown")
                .Set(ai => ai.LastModificationTime, DateTime.UtcNow)
                .Set(ai => ai.HasFreshIndex, false)
                .Unset(ai => ai.TagNamespaces[ns]);

            return await Mongo.Accounts.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<AccountData> { IsUpsert = false, ReturnDocument = ReturnDocument.After });
        }

        public async Task<AccountData> RemoveTagIfNotPositive(string tenantId, string accountId, string ns, string tag)
        {
            var filter = Builders<AccountData>.Filter.And(
                Builders<AccountData>.Filter.Eq(ai => ai.TenantId, tenantId),
                Builders<AccountData>.Filter.Eq(ai => ai.AccountId, accountId),
                Builders<AccountData>.Filter.Lte(ai => ai.TagNamespaces[ns][tag], 0d)
            );

            var update = Builders<AccountData>.Update
                .Set(ai => ai.LastModifiedBy, "unknown")
                .Set(ai => ai.LastModificationTime, DateTime.UtcNow)
                .Set(ai => ai.HasFreshIndex, false)
                .Unset(ai => ai.TagNamespaces[ns][tag]);

            return await Mongo.Accounts.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<AccountData> { IsUpsert = false, ReturnDocument = ReturnDocument.After });
        }

        public async Task<List<string>> FetchAccountIds(int batchSize, string tenantId, 
            string lowerBound, bool lowerBoundInclusive, string upperBound, bool upperBoundInclusive)
        {
            var filterItems = new List<FilterDefinition<AccountData>>
            {
                Builders<AccountData>.Filter.Eq(ad => ad.TenantId, tenantId)
            };

            if (!string.IsNullOrEmpty(lowerBound))
                filterItems.Add(lowerBoundInclusive
                    ? Builders<AccountData>.Filter.Gte(ad => ad.AccountId, lowerBound)
                    : Builders<AccountData>.Filter.Gt(ad => ad.AccountId, lowerBound));

            if (!string.IsNullOrEmpty(upperBound))
                filterItems.Add(upperBoundInclusive
                    ? Builders<AccountData>.Filter.Lte(ad => ad.AccountId, upperBound)
                    : Builders<AccountData>.Filter.Lt(ad => ad.AccountId, upperBound));

            var queryResult = await Mongo.Accounts.FindAsync(
                Builders<AccountData>.Filter.And(filterItems),
                new FindOptions<AccountData, string>
                {
                    Limit = batchSize,
                    Projection = Builders<AccountData>.Projection.Expression(ad => ad.AccountId),
                    Sort = Builders<AccountData>.Sort.Ascending(ad => ad.AccountId)
                });

            var allData = await queryResult.ToListAsync();

            return allData;
        }
    }
}