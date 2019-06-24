using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using Tesseract.Common.Extensions;
using Tesseract.Core.Connection;
using Tesseract.Core.Index.Model;

namespace Tesseract.Core.Index.Implementation
{
    public class EsAccountIndexWriter : IAccountIndexWriter
    {

        private readonly IEsManager EsManager;
        public EsAccountIndexWriter(IEsManager esManager)
        {
            EsManager = esManager;
        }

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