using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.ApiModel.Tags;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Storage
{
    [Contract]
    public interface ITagNsDefinitionStore
    {
        Task<List<TagNsDefinitionData>> LoadAll(string tenantId);
        Task<TagNsDefinitionData> Load(string tenantId, string ns);
        Task<TagNsDefinitionData> AddOrUpdate(string tenantId, string ns, PutTagNsDefinitionRequest data);
        Task<TagNsDefinitionData> Remove(string tenantId, string ns);
    }
}