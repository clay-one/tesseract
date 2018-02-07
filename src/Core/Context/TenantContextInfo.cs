using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Context
{
    public class TenantContextInfo
    {
        public bool Initialized { get; private set; }

        public string Id { get; }

        public ReadOnlyDictionary<string, TagNsDefinitionData> TagNsDefinitions { get; private set; }
        public ReadOnlyDictionary<string, FieldDefinitionData> FieldDefinitions { get; private set; }

        public TenantContextInfo(string id)
        {
            Id = id;
            Initialized = false;
        }

        public async Task Initialize(IComposer composer)
        {
            var tagNsStore = composer.GetComponent<ITagNsDefinitionStore>();
            var fieldStore = composer.GetComponent<IFieldDefinitionStore>();

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