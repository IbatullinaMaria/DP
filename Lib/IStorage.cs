using System.Collections.Generic;

namespace Valuator
{
    public interface IStorage
    {
        void StoreValue(string key, string value);
        string GetValue(string key);
        bool FindText(string text);
        void StoreTextToSet(string text);
    }
}
