using System;
using System.Threading.Tasks;
using CommandLine;
using Tesseract.Cli.Adminitrator.Base;

namespace Tesseract.Cli.Adminitrator.ReindexAll
{
    [Verb("reindex-all", HelpText = "Starts re-indexing all of the account data.")]
    public class ReindexAllVerb : VerbBase
    {
        private string _jobId;

        protected override int ProgressReportDelayMillis => 3000;

        protected override Task<bool> ValidateParametersAsync()
        {
            return Task.FromResult(true);
        }

        protected override Task PrepareAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task StartJobAsync()
        {
            var result = await Client.Jobs.StartReindexingAllAccounts();
            _jobId = result.JobId;

            Console.WriteLine($"Reindexing started. JobId: {_jobId}");
        }

        protected override async Task<bool> ReportProgressAsync()
        {
            var taskStatus = await Client.Jobs.GetJobStatus(_jobId);

            Console.WriteLine(
                $"State: {taskStatus.State}, " +
                $"Queue: {taskStatus.QueueLength}, " +
                $"Processed: {taskStatus.ItemsProcessed}, " +
                $"Failed: {taskStatus.ItemsFailed}, " +
                $"Requeued: {taskStatus.ItemsRequeued}, " +
                $"Generated: {taskStatus.ItemsGeneratedForTargetQueue}, " +
                $"Exc: {taskStatus.ExceptionCount}");

            Console.WriteLine(
                $"Seconds since last " +
                $"activity: {(int) (taskStatus.LastActivityTime - DateTime.Now).TotalSeconds}, " +
                $"process: {(int) (taskStatus.LastProcessTime - DateTime.Now).TotalSeconds}, " +
                $"health check: {(int) (taskStatus.LastHealthCheckTime - DateTime.Now).TotalSeconds}");

            Console.WriteLine();

            return taskStatus.IsCompleted;
        }
    }
}