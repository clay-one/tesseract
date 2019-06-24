using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Tesseract.Core.Connection.Implementation
{
    public class DefaultRedisManager : IRedisManager
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.GetDatabase();
        }

        public ISubscriber GetSubscriber()
        {
            return _connectionMultiplexer.GetSubscriber();
        }

        public DefaultRedisManager(IOptions<RedisConfig> options)
        {
            var address = options.Value.Address;
            var configurationOptions = ConfigurationOptions.Parse(address);
            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        }
    }
}