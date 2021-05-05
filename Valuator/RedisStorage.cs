using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly IDatabase db;
        private string _host = "localhost";

        public RedisStorage() {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}, allowAdmin=true");
            db = connectionMultiplexer.GetDatabase();
        }

        public string LoadValue(string key)
        {
            return db.StringGet(key);
        }

        public bool ExistingText(string setKey, string value)
        {
            return db.SetContains(setKey, value);
        }

        public void StoreValue(string key, string value)
        {
            db.StringSet(key, value);
        }

        public void StoreToSet(string setKey, string value)
        {
            db.SetAdd(setKey, value);
        }
    }
}