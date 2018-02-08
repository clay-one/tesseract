using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using Tesseract.Common.ComposerImposter;
using Tesseract.Common.Extensions;
using Tesseract.Core.Connection;
using Tesseract.Core.Index.Model;

namespace Tesseract.Core.Index.Implementation
{
    [Component]
    public class EsAccountIndexWriter : IAccountIndexWriter
    {
        [ComponentPlug]
        public IEsManager EsManager { get; set; }

        public async Task Index(string tenantId, List<AccountIndexModel> models)
        {
            if (!models.SafeAny())
            {
                return;
            }

            await EsManager.Client.IndexManyAsync(models, EsManager.GetTenantIndexName(tenantId));
        }
    }
}