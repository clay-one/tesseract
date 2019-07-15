using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Appson.Tesseract.Web.Mappings;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Jobs;
using Tesseract.Core.Logic;

namespace Appson.Tesseract.Web.Controllers
{
    public class JobInfoController : ApiControllerBase
    {
        // GET	/jobs/list	-B, SAFE	Get the list of running / pending / recently finished jobs
        // GET	/jobs/j/:j	-B, SAFE Enquiry the status / result of an asynchronous operation, using the ID returned when starting the job
        // GET	/jobs/push/accounts	-B, SAFE	Returns the list of ongoing or recently finished push jobs on accounts
        // GET	/jobs/push/changes	-B, SAFE Returns the list of ongoing or recently finished push jobs on changes


        private readonly IJobLogic _jobLogic;

        public JobInfoController(IJobLogic jobLogic)
        {
            _jobLogic = jobLogic;
        }

        [HttpGet("jobs/list")]
        public async Task<IActionResult> GetJobList()
        {
            var allJobs = await _jobLogic.LoadAllJobs();
            return Ok(new GetJobListResponse
            {
                Jobs = allJobs.Select(j => j.ToGetJobListResponseItem()).ToList()
            });
        }

        [HttpGet("jobs/j/{jobId}")]
        public async Task<IActionResult> GetJobStatus(string jobId)
        {
            var jobData = await _jobLogic.LoadJob(jobId);
            if (jobData == null)
                return NotFound();

            var result = jobData.ToGetJobStatusResponse();

            // TODO Multiple-loading of job data when calling this API. Once above, and once in getting queue length
            result.QueueLength = await _jobLogic.GetQueueLength(jobId);

            return Ok(result);
        }

        [HttpGet("jobs/push/accounts")]
        public IActionResult GetAccountPushJobList()
        {
            return NotImplemented();
        }

        [HttpGet("jobs/push/changes")]
        public IActionResult GetChangePushJobList()
        {
            return NotImplemented();
        }
    }
}
