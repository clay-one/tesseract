using System.Threading.Tasks;
using ServiceStack.Text;
using Tesseract.Core.Connection;

namespace Tesseract.Core.Job.Implementation
{
    public class RedisJobNotification : IJobNotification
    {
        private const string JobUpdatedChannelName = "job-updated";
        //        private Thread _targetThread;

        private readonly IRedisManager _redisManager;

        private readonly IJobNotificationTarget _notificationTarget;

        public RedisJobNotification(IRedisManager redisManager, IJobNotificationTarget jobNotificationTarget)
        {
            _redisManager = redisManager;
            _notificationTarget = jobNotificationTarget;
        }

        public Task StartNotificationTargetThread()
        {
            return _redisManager.GetSubscriber().SubscribeAsync(JobUpdatedChannelName, (channel, value) =>
            {
                $"Got notification on JobId '{value}' from channel '{channel}'".Print();
                _notificationTarget.ProcessNotification(value).GetAwaiter().GetResult();
            });
        }

        public Task StopNotificationTargetThread()
        {
            return _redisManager.GetSubscriber().UnsubscribeAsync(JobUpdatedChannelName);
        }

        public async Task NotifyJobUpdated(string jobId)
        {
            await _redisManager.GetSubscriber().PublishAsync(JobUpdatedChannelName, jobId);
        }
    }
}