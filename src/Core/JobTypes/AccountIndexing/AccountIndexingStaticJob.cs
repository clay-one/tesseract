using System.Threading.Tasks;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Job;
using Tesseract.Core.Logic;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.JobTypes.AccountIndexing
{
    [Component]
    public class AccountIndexingStaticJob : IStaticJob
    {
        [ComponentPlug]
        public IJobManager JobManager { get; set; }
        
        [ComponentPlug]
        public ICurrentTenantLogic Tenant { get; set; }
        
        public async Task EnsureJobsDefined()
        {
            await JobManager.CreateNewJobOrUpdateDefinition<AccountIndexingStep>(
                Tenant.None, "account-indexer", nameof(AccountIndexingStep), new JobConfigurationData
                {
                    MaxBatchSize = 100,
                    MaxConcurrentBatchesPerWorker = 5,
                    IsIndefinite = true,
                    MaxBlockedSecondsPerCycle = 300
                });

            await JobManager.StartJobIfNotStarted(Tenant.None, nameof(AccountIndexingStep));
        }
    }
}