using System.Collections.Generic;

namespace Valuator
{
    public interface IStorage
    {
        void StoreNewShardKey(string key, string value);
        void StoreValue(string shardKey, string key, string value);
        string GetValue(string shardKey, string key);
        bool FindText(string text);
        void StoreTextToSet(string shardKey, string text);

        string GetShardId(string key);
    }
}
