using System.Threading.Tasks;
using Tesseract.Core.Job;
using Tesseract.Core.Logic;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.JobTypes.AccountIndexing
{
    public class AccountIndexingStaticJob : IStaticJob
    {
        private readonly IJobManager _jobManager;

        private readonly ICurrentTenantLogic _tenant;

        public AccountIndexingStaticJob(IJobManager jobManager, ICurrentTenantLogic currentTenantLogic)
        {
            _jobManager = jobManager;
            _tenant = currentTenantLogic;
        }

        public async Task EnsureJobsDefined()
        {
            await _jobManager.CreateNewJobOrUpdateDefinition<AccountIndexingStep>(
                _tenant.None, "account-indexer", nameof(AccountIndexingStep), new JobConfigurationData
                {
                    MaxBatchSize = 100,
                    MaxConcurrentBatchesPerWorker = 5,
                    IsIndefinite = true,
                    MaxBlockedSecondsPerCycle = 300
                });

            await _jobManager.StartJobIfNotStarted(_tenant.None, nameof(AccountIndexingStep));
        }
    }
}