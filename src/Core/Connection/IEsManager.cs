using System.Threading.Tasks;
using Nest;
using Tesseract.Common.ComposerImposter;

namespace Tesseract.Core.Connection
{
    [Contract]
    public interface IEsManager
    {
        string GetTenantIndexName(string tenantId);
        Task DeleteTenantIndex(string tenantId);
        Task CreateTenantIndex(string tenantId);
        Task EnsureTenantIndex(string tenantId);
        Task EnsureIndexTagNsAndFieldMappings(string tenantId);

        Task SetTagNsMapping(string tenantId, string ns);
        Task SetFieldMapping(string tenantId, string fieldName);

        ElasticClient Client { get; }
    }
}