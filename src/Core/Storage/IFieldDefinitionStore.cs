using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.ApiModel.Fields;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Storage
{
    public interface IFieldDefinitionStore
    {
        Task<List<FieldDefinitionData>> LoadAll(string tenantId);
        Task<FieldDefinitionData> Load(string tenantId, string fieldName);
        Task<FieldDefinitionData> AddOrUpdate(string tenantId, string fieldName, PutFieldDefinitionRequest data);
        Task<FieldDefinitionData> Remove(string tenantId, string fieldName);
    }
}