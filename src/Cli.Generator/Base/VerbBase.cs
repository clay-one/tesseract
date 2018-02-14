using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Tesseract.Client;
using Tesseract.Common.Utils;

namespace Tesseract.Cli.Generator.Base
{
    public abstract class VerbBase
    {
        private long _operationCount;
        protected TesseractClient Client { get; private set; }

        protected long OperationCount => Interlocked.Read(ref _operationCount);

        public void Run()
        {
            Client = new TesseractClient(ServerUri);

            if (!ValidateParametersAsync().GetAwaiter().GetResult())
                return;

            PrepareAsync().GetAwaiter().GetResult();
            StartAsync().GetAwaiter().GetResult();
        }

        protected virtual Task<bool> ValidateParametersAsync()
        {
            if (ThreadCount < 1 || ThreadCount > 100)
            {
                Console.WriteLine("ERROR: Thread count should be between 1 and 100.");
                return Task.FromResult(false);
            }

            if (MinBatchSize < 1)
            {
                Console.WriteLine("ERROR: Minimum batch size must be more than or equal to 1.");
                return Task.FromResult(false);
            }

            if (MaxBatchSize < MinBatchSize)
                MaxBatchSize = MinBatchSize;

            return Task.FromResult(true);
        }

        protected virtual Task PrepareAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual async Task StartAsync()
        {
            for (var i = 0; i < ThreadCount; i++)
            {
                var t = CreateNewThread();
                t.Start();
            }

            var startTime = DateTime.UtcNow;
            var timeBefore = DateTime.UtcNow;
            var countBefore = 0L;

            while (true)
            {
                await Task.Delay(1000);

                var timeAfter = DateTime.UtcNow;
                var countAfter = OperationCount;

                var increment = countAfter - countBefore;
                var seconds = (timeAfter - timeBefore).TotalSeconds;
                var secondsSinceStart = (timeAfter - startTime).TotalSeconds;

                ReportProgress(countAfter, increment, seconds, secondsSinceStart);

                timeBefore = timeAfter;
                countBefore = countAfter;
            }

            // ReSharper disable once FunctionNeverReturns
        }

        protected virtual void ReportProgress(long countAfter, long increment, double seconds, double secondsSinceStart)
        {
            Console.WriteLine($"Total operations: {countAfter,7}, " +
                              $"Speed: {increment / seconds,6:N1} op/sec, " +
                              $"Avg. Speed: {countAfter / secondsSinceStart,6:N1} op/sec");
        }

        protected virtual Thread CreateNewThread()
        {
            return new Thread(async () =>
            {
                while (true)
                {
                    var numOperations = await ExecuteNextBatch();
                    Interlocked.Add(ref _operationCount, numOperations);
                }

                // ReSharper disable once FunctionNeverReturns
            });
        }

        protected abstract Task<long> ExecuteNextBatch();

        protected virtual int GetNextBatchSize()
        {
            return RandomProvider.GetThreadRandom().Next(MinBatchSize, MaxBatchSize + 1);
        }

        #region Command-line options

        [Value(0, Required = true, HelpText = "Base url of the Tesseract server. Eg. http://localhost:2771/")]
        public Uri ServerUri { get; set; }

        [Option("thread-count", Default = 10, HelpText = "Number of threads that concurrently call the server.")]
        public int ThreadCount { get; set; }

        [Option('b', "min-batch-size", Default = 20,
            HelpText = "Minimum size of each batch of accounts to send to server at once.")]
        public int MinBatchSize { get; set; }

        [Option("max-batch-size", Default = -1,
            HelpText =
                "Maximum size of each batch of accounts to send to server at once. If not set, minimum batch size is used for all requests.")]
        public int MaxBatchSize { get; set; }

        #endregion
    }
}