using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Jobs;
using Tesseract.Core.Job;
using Tesseract.Core.Logic;

namespace Appson.Tesseract.Web.Controllers
{
    public class ReindexJobController : ApiControllerBase
    {
        // POST	/jobs/reindex/accounts/all	-B, JOB	Starts re-indexing all known accounts
        // POST	/jobs/reindex/accounts/by/query	+B, JOB Runs a query on accounts and starts re-indexing the resulting accounts
        // POST	/jobs/reindex/accounts/by/tags/ns/:ns	-B, JOB Starts re-indexing all accounts tagged in a specific namespace
        // POST /jobs/reindex/accounts/by/tags/ns/:ns/t/:t	-B, JOB Starts re-indexing all accounts with a specific tag


        private readonly IJobLogic _jobLogic;
        private readonly IJobManager _jobManager;
        private readonly ICurrentTenantLogic _tenant;

        public ReindexJobController(IJobLogic jobLogic,
            IJobManager jobManager,
            ICurrentTenantLogic currentTenantLogic)
        {
            _jobManager = jobManager;
            _jobLogic = jobLogic;
            _tenant = currentTenantLogic;
        }

        /// <summary>
        /// POST
        /// /jobs/reindex/accounts/all
        /// -B, JOB
        /// Starts re-indexing all known accounts
        /// </summary>
        [HttpPost("jobs/reindex/accounts/all")]
        public async Task<IActionResult> StartReindexingAllAccounts()
        {
            var jobId = await _jobLogic.CreateReindexAllJob();
            await _jobManager.StartJob(_tenant.Id, jobId);

            return Ok(new StartReindexingAllAccountsResponse
            {
                JobId = jobId
            });
        }

        /// <summary>
        /// POST
        /// /jobs/reindex/accounts/by/query
        /// +B, JOB
        /// Runs a query on accounts and starts re-indexing the resulting accounts
        /// </summary>
        [HttpPost("jobs/reindex/accounts/by/query")]
        public IActionResult StartReindexingAccountsByQuery(string input)
        {
            return NotImplemented();
        }

        /// <summary>
        /// POST
        /// /jobs/reindex/accounts/by/tags/ns/:ns
        /// -B, JOB
        /// Starts re-indexing all accounts tagged in a specific namespace
        /// </summary>
        [HttpPost("jobs/reindex/accounts/by/tags/ns/{ns}")]
        public IActionResult StartReindexingAccountsByTagNamespace(string ns)
        {
            return NotImplemented();
        }

        /// <summary>
        /// POST
        /// /jobs/reindex/accounts/by/tags/ns/:ns/t/:t
        /// -B, JOB
        /// Starts re-indexing all accounts with a specific tag
        /// </summary>
        [HttpPost("jobs/reindex/accounts/by/tags/ns/{ns}/t/{tag}")]
        public IActionResult StartReindexingAccountsByTag(string ns, string tag)
        {
            return NotImplemented();
        }
    }
}
