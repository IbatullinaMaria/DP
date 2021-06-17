using StackExchange.Redis;
using System.Collections.Generic;
using System;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private const string _host = "localhost";
        private const string _allTextsKey = "allTextsKey";
        private readonly IConnectionMultiplexer _connection;
        public RedisStorage()
        {
            _connection = ConnectionMultiplexer.Connect($"{_host}, allowAdmin=true");
        }
        public void StoreValue(string key, string value)
        {
            var db = _connection.GetDatabase();
            db.StringSet(key, value);
        }
        public void StoreTextToSet(string value)
        {
            var db = _connection.GetDatabase();
            db.SetAdd(_allTextsKey, value);
        }

        public string GetValue(string key)
        {    
            var db = _connection.GetDatabase();
            return db.StringGet(key);
        }
        public bool FindText(string text)
        {
            var db = _connection.GetDatabase();
            return db.SetContains(_allTextsKey, text);
        } 
    }
}