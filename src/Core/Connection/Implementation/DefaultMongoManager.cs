using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Connection.Implementation
{
    [Component]
    public class DefaultMongoManager : IMongoManager
    {
        public IMongoDatabase Database { get; private set; }
        public IMongoCollection<AccountData> Accounts { get; private set; }
        public IMongoCollection<FieldDefinitionData> FieldDefinitions { get; private set; }
        public IMongoCollection<TagNsDefinitionData> TagNsDefinitions { get; private set; }
        public IMongoCollection<JobData> Jobs { get; private set; }

        [ConfigurationPoint("mongo.clientSettings")]
        public MongoClientSettings ClientSettings { get; set; }

        [ConfigurationPoint("mongo.databaseName")]
        public string DatabaseName { get; set; }
        
        [ConfigurationPoint("mongo.databaseSettings")]
        public MongoDatabaseSettings DatabaseSettings { get; set; }
        
        [OnCompositionComplete]
        public void OnCompositionComplete()
        {
            var client = new MongoClient(ClientSettings);
            Database = client.GetDatabase(DatabaseName, DatabaseSettings);

            Accounts = Database.GetCollection<AccountData>(nameof(AccountData));
            FieldDefinitions = Database.GetCollection<FieldDefinitionData>(nameof(FieldDefinitionData));
            TagNsDefinitions = Database.GetCollection<TagNsDefinitionData>(nameof(TagNsDefinitionData));
            Jobs = Database.GetCollection<JobData>(nameof(JobData));
        }

        public void DeleteTenantData(string tenantId)
        {
            Accounts.DeleteMany(ad => ad.TenantId == tenantId);
            FieldDefinitions.DeleteMany(fd => fd.TenantId == tenantId);
            TagNsDefinitions.DeleteMany(td => td.TenantId == tenantId);
            Jobs.DeleteMany(jd => jd.TenantId == tenantId);
        }
    }
}