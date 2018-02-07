using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Connection
{
    [Contract]
    public interface IMongoManager
    {
        IMongoDatabase Database { get; }

        IMongoCollection<AccountData> Accounts { get; }
        IMongoCollection<FieldDefinitionData> FieldDefinitions { get; }
        IMongoCollection<TagNsDefinitionData> TagNsDefinitions { get; }
        IMongoCollection<JobData> Jobs { get; }

        void DeleteTenantData(string tenantId);
    }
}