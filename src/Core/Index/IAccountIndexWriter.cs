using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.Core.Index.Model;

namespace Tesseract.Core.Index
{
    public interface IAccountIndexWriter
    {
        Task Index(string tenantId, List<AccountIndexModel> indexModels);
    }
}