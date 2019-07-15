using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tesseract.Core.Logic
{
    public interface ITaggingLogic
    {
        Task AddTag(string accountId, string ns, string tag);
        Task SetTagWeight(string accountId, string ns, string tag, double weight);
        Task ReplaceTagNs(string accountId, string ns, IEnumerable<KeyValuePair<string, double>> tags);
    }
}