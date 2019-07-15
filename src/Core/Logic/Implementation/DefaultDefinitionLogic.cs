using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.ApiModel.Fields;
using Tesseract.ApiModel.Tags;
using Tesseract.Core.Connection;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Logic.Implementation
{
    public class DefaultDefinitionLogic : IDefinitionLogic
    {
        public ITagNsDefinitionStore TagNsDefinitionStore { get; set; }

        public IFieldDefinitionStore FieldDefinitionStore { get; set; }

        public IEsManager EsManager { get; set; }

        private readonly ICurrentTenantLogic Tenant;

        public DefaultDefinitionLogic(ITagNsDefinitionStore tagNsDefinitionStore,
            IFieldDefinitionStore fieldDefinitionStore,
            IEsManager esManager, ICurrentTenantLogic tenantLogic)
        {
            TagNsDefinitionStore = tagNsDefinitionStore;
            FieldDefinitionStore = fieldDefinitionStore;
            EsManager = esManager;
            Tenant = tenantLogic;
        }

        public async Task<List<TagNsDefinitionData>> LoadAllNsDefinitions()
        {
            return await TagNsDefinitionStore.LoadAll(Tenant.Id);
        }

        public async Task<TagNsDefinitionData> LoadNsDefinition(string ns)
        {
            return await TagNsDefinitionStore.Load(Tenant.Id, ns);
        }

        public async Task AddOrUpdateTagNsDefinition(string ns, PutTagNsDefinitionRequest request)
        {
            var currentDefinition = await TagNsDefinitionStore.Load(Tenant.Id, ns);
            if (currentDefinition == null) await EsManager.SetTagNsMapping(Tenant.Id, ns);

            await TagNsDefinitionStore.AddOrUpdate(Tenant.Id, ns, request);
        }

        public async Task<List<FieldDefinitionData>> LoadAllFieldDefinitions()
        {
            return await FieldDefinitionStore.LoadAll(Tenant.Id);
        }

        public async Task<FieldDefinitionData> LoadFieldDefinition(string fieldName)
        {
            return await FieldDefinitionStore.Load(Tenant.Id, fieldName);
        }

        public async Task AddOrUpdateFieldDefinition(string fieldName, PutFieldDefinitionRequest request)
        {
            var currentDefinition = await FieldDefinitionStore.Load(Tenant.Id, fieldName);
            if (currentDefinition == null) await EsManager.SetFieldMapping(Tenant.Id, fieldName);

            await FieldDefinitionStore.AddOrUpdate(Tenant.Id, fieldName, request);
        }
    }
}