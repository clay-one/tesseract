using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Tesseract.Client;

namespace Tesseract.Cli.Adminitrator.Base
{
    public abstract class VerbBase
    {
        private long _operationCount;
        protected TesseractClient Client { get; private set; }

        protected long OperationCount => Interlocked.Read(ref _operationCount);

        protected virtual int ProgressReportDelayMillis => 1000;

        #region Command-line options

        [Value(0, Required = true, HelpText = "Base url of the Tesseract server. Eg. http://localhost:2771/")]
        public Uri ServerUri { get; set; }

        #endregion

        public void Run()
        {
            Client = new TesseractClient(ServerUri);

            if (!ValidateParametersAsync().GetAwaiter().GetResult())
                return;

            PrepareAsync().GetAwaiter().GetResult();
            StartAsync().GetAwaiter().GetResult();
        }

        protected abstract Task<bool> ValidateParametersAsync();

        protected abstract Task PrepareAsync();

        protected virtual async Task StartAsync()
        {
            await StartJobAsync();
            var done = await ReportProgressAsync();

            while (!done)
            {
                await Task.Delay(ProgressReportDelayMillis);
                done = await ReportProgressAsync();
            }

            Console.WriteLine("DONE.");
        }

        protected abstract Task StartJobAsync();

        protected abstract Task<bool> ReportProgressAsync();
    }
}