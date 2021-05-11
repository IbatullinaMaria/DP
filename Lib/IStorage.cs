using System.Collections.Generic;

namespace Valuator
{
    public interface IStorage
    {
        void StoreValue(string key, string value);
        string LoadValue(string key);
        bool ExistingText(string setKey, string value);
        void StoreToSet(string setKey, string value);
    }
}