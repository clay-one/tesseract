using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tesseract.Core.Distribution.Implementation
{
    [Component]
    public class RedisBasedDistributionNodes : IDistributionNodes
    {
        public Task StartHeartbeat()
        {
            return Task.CompletedTask;
        }

        public Task StopHeartbeat()
        {
            return Task.CompletedTask;
        }

        public Task<List<string>> GetAllAliveNodes()
        {
            return Task.FromResult(new List<string>());
        }

        public Task<List<string>> GetAliveWebNodes()
        {
            return Task.FromResult(new List<string>());
        }

        public Task<List<string>> GetAliveWorkerNodes()
        {
            return Task.FromResult(new List<string>());
        }
    }
}