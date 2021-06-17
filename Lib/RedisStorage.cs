using StackExchange.Redis;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private const string _host = "localhost";
        private readonly string _hostEu;
        private readonly string _hostRu;
        private readonly string _hostOther;

        private const string _allTextsKey = "allTextsKey";
        private readonly IConnectionMultiplexer _connection;
        private readonly IEnumerable<ShardInfo> _shards;
        public RedisStorage()
        {
            _connection = ConnectionMultiplexer.Connect($"{_host}, allowAdmin=true");
            _hostEu = Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User);
            _hostRu = Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User);
            _hostOther = Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User);

            _shards = GetShards();         
        }
        public void StoreValue(string shardKey, string key, string value)
        {
            var conn = GetConnectionByShardId(shardKey);
            var db = conn.GetDatabase();
            db.StringSet(key, value);
        }

        public void StoreNewShardKey(string key, string value)
        {
            var db = _connection.GetDatabase();
            db.StringSet(key, value);
        }
        
        public string GetShardId(string key)
        {
            var db = _connection.GetDatabase();
            return db.StringGet(key);
        }
        public void StoreTextToSet(string shardKey, string value)
        {
            var conn = GetConnectionByShardId(shardKey);
            var db = conn.GetDatabase();
            db.SetAdd(_allTextsKey, value);
        }

        public string GetValue(string shardKey, string key)
        {   
            var conn = GetConnectionByShardId(shardKey);
            var db = conn.GetDatabase();
            return db.StringGet(key);
        }
        public bool FindText(string text)
        {
            foreach(var shard in _shards)
            {
                var db = shard.Conn.GetDatabase();
                if (db.SetContains(_allTextsKey, text))
                {
                    return true;
                }
            }
            return false;
        }

        private IConnectionMultiplexer GetConnectionByShardId(string id)
        {
            var db = _connection.GetDatabase();
            var shardId = db.StringGet(id);

            foreach (var shard in _shards)
            {
                if (shardId == shard.ShardId)
                {
                    return shard.Conn;
                }
            }
            return _connection;
        }
        private IEnumerable<ShardInfo> GetShards()
        {
            return new[]
            {
                new ShardInfo
                {
                    ShardId = Constants.RuCountrySegment,
                    Conn = ConnectionMultiplexer.Connect(_hostRu)

                },
                new ShardInfo
                {
                    ShardId = Constants.EuCountriesSegment,
                    Conn = ConnectionMultiplexer.Connect(_hostEu)
                },
                new ShardInfo
                {
                    ShardId = Constants.OtherCountriesSegment,
                    Conn =  ConnectionMultiplexer.Connect(_hostOther)
                }
            };
        }      
    }
}