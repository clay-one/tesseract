using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;
using ServiceProviderServiceExtensions = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions;

namespace Tesseract.Core.Context
{
    public class TenantContextInfo
    {
        public TenantContextInfo(string id)
        {
            Id = id;
            Initialized = false;
        }

        public bool Initialized { get; private set; }

        public string Id { get; }

        public ReadOnlyDictionary<string, TagNsDefinitionData> TagNsDefinitions { get; private set; }
        public ReadOnlyDictionary<string, FieldDefinitionData> FieldDefinitions { get; private set; }

        public async Task Initialize(IServiceProvider serviceProvider)
        {
            var tagNsStore = ServiceProviderServiceExtensions.GetRequiredService<ITagNsDefinitionStore>(serviceProvider);
            var fieldStore = ServiceProviderServiceExtensions.GetRequiredService<IFieldDefinitionStore>(serviceProvider);

            var allTagNss = await tagNsStore.LoadAll(Id);
            TagNsDefinitions = new ReadOnlyDictionary<string, TagNsDefinitionData>(
                allTagNss.ToDictionary(nsd => nsd.Namespace));

            var allFields = await fieldStore.LoadAll(Id);
            FieldDefinitions = new ReadOnlyDictionary<string, FieldDefinitionData>(
                allFields.ToDictionary(fd => fd.FieldName));

            Initialized = true;
        }
    }
}