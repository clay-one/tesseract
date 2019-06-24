using StackExchange.Redis;

namespace Tesseract.Core.Connection
{
    public interface IRedisManager
    {
        IDatabase GetDatabase();
        ISubscriber GetSubscriber();
    }
}