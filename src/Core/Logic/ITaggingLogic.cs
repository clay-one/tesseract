using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;

namespace Tesseract.Core.Logic
{
    [Contract]
    public interface ITaggingLogic
    {
        Task AddTag(string accountId, string ns, string tag);
        Task SetTagWeight(string accountId, string ns, string tag, double weight);
        Task ReplaceTagNs(string accountId, string ns, IEnumerable<KeyValuePair<string, double>> tags);
    }
}