using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Connection.Implementation
{
    public class DefaultMongoManager : IMongoManager
    {

        public IMongoDatabase Database { get; private set; }
        public IMongoCollection<AccountData> Accounts { get; private set; }
        public IMongoCollection<FieldDefinitionData> FieldDefinitions { get; private set; }
        public IMongoCollection<TagNsDefinitionData> TagNsDefinitions { get; private set; }
        public IMongoCollection<JobData> Jobs { get; private set; }

        public void DeleteTenantData(string tenantId)
        {
            Accounts.DeleteMany(ad => ad.TenantId == tenantId);
            FieldDefinitions.DeleteMany(fd => fd.TenantId == tenantId);
            TagNsDefinitions.DeleteMany(td => td.TenantId == tenantId);
            Jobs.DeleteMany(jd => jd.TenantId == tenantId);
        }

        public DefaultMongoManager(IOptions<MongoDbConfig> options)
        {
            var address = options.Value.Address;
            var mongoUri = new MongoUrl(address);


            var client = new MongoClient(mongoUri);

            Database = client.GetDatabase(mongoUri.DatabaseName);

            Accounts = Database.GetCollection<AccountData>(nameof(AccountData));
            FieldDefinitions = Database.GetCollection<FieldDefinitionData>(nameof(FieldDefinitionData));
            TagNsDefinitions = Database.GetCollection<TagNsDefinitionData>(nameof(TagNsDefinitionData));
            Jobs = Database.GetCollection<JobData>(nameof(JobData));
        }
    }
}