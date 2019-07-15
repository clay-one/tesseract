using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.ApiModel.Fields;
using Tesseract.ApiModel.Tags;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Logic
{
    public interface IDefinitionLogic
    {
        Task<List<TagNsDefinitionData>> LoadAllNsDefinitions();
        Task<TagNsDefinitionData> LoadNsDefinition(string ns);
        Task AddOrUpdateTagNsDefinition(string ns, PutTagNsDefinitionRequest request);

        Task<List<FieldDefinitionData>> LoadAllFieldDefinitions();
        Task<FieldDefinitionData> LoadFieldDefinition(string fieldName);
        Task AddOrUpdateFieldDefinition(string fieldName, PutFieldDefinitionRequest request);
    }
}