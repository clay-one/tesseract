﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Logic
{
    [Contract]
    public interface IJobLogic
    {
        Task<List<JobData>> LoadAllJobs();
        Task<JobData> LoadJob(string jobId);
        Task<long> GetQueueLength(string jobId);
        
        Task<string> CreateReindexAllJob();
        Task<string> CreateFetchAccountsJob(AccountQuery query, PushBehaviorSpecification behavior, 
            string displayName, string targetJobId, long estimatedCount);
        Task<string> CreatePushAccountsJob(PushBehaviorSpecification behavior, PushTargetSpecification target, 
            string displayName, long estimatedCount);

    }
}