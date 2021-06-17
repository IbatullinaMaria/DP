using StackExchange.Redis;

namespace Valuator
{
    public class ShardInfo
    {
        public string ShardId;
        public IConnectionMultiplexer Conn;
    }
}
