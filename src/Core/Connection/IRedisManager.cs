using StackExchange.Redis;
using Tesseract.Common.ComposerImposter;

namespace Tesseract.Core.Connection
{
    [Contract]
    public interface IRedisManager
    {
        IDatabase GetDatabase();
        ISubscriber GetSubscriber();
    }
}