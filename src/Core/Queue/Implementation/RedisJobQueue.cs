using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;
using Tesseract.Core.Connection;

namespace Tesseract.Core.Queue.Implementation
{
    public class RedisJobQueue<TItem> : IJobQueue<TItem> where TItem : JobStepBase
    {
        private readonly IRedisManager _redisManager;

        public RedisJobQueue(IRedisManager redisManager)
        {
            _redisManager = redisManager;
        }

        public Task EnsureJobQueueExists(string jobId = null)
        {
            // Redis lists are created upon adding first item, so nothing to do here.
            return Task.CompletedTask;
        }

        public async Task<long> GetQueueLength(string jobId = null)
        {
            return await _redisManager.GetDatabase().ListLengthAsync(GetRedisKey(jobId));
        }

        public async Task PurgeQueueContents(string jobId = null)
        {
            await _redisManager.GetDatabase().KeyDeleteAsync(GetRedisKey(jobId));
        }

        public async Task Enqueue(TItem item, string jobId = null)
        {
            await _redisManager.GetDatabase().ListLeftPushAsync(GetRedisKey(jobId), item.ToJson());
        }

        public async Task EnqueueBatch(IEnumerable<TItem> items, string jobId = null)
        {
            var redisKey = GetRedisKey(jobId);
            var redisDb = _redisManager.GetDatabase();
            var tasks = items.Select(item => redisDb.ListLeftPushAsync(redisKey, item.ToJson()));
            await Task.WhenAll(tasks);
        }

        public async Task<TItem> Dequeue(string jobId = null)
        {
            string serialized = await _redisManager.GetDatabase().ListRightPopAsync(GetRedisKey(jobId));
            return serialized.FromJson<TItem>();
        }

        public async Task<IEnumerable<TItem>> DequeueBatch(int maxBatchSize, string jobId = null)
        {
            if (maxBatchSize < 1 || maxBatchSize > 10000)
            {
                throw new ArgumentException("MaxBatchSize is out of range");
            }

            var redisKey = GetRedisKey(jobId);
            var redisDb = _redisManager.GetDatabase();
            var tasks = Enumerable
                .Range(1, maxBatchSize)
                .Select(i => redisDb.ListRightPopAsync(redisKey));

            var results = await Task.WhenAll(tasks);
            return results
                .Select(r => (string) r)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.FromJson<TItem>());
        }

        #region Private helper methods

        private string GetRedisKey(string jobId)
        {
            return "job_" + (string.IsNullOrEmpty(jobId) ? typeof(TItem).Name : jobId);
        }

        #endregion
    }
}