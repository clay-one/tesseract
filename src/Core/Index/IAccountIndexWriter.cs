using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Index.Model;

namespace Tesseract.Core.Index
{
    [Contract]
    public interface IAccountIndexWriter
    {
        Task Index(string tenantId, List<AccountIndexModel> indexModels);
    }
}