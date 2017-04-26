using System.Collections.Generic;

namespace JIRAIntegration
{
    interface IItemCache<TKey,TValue> 
    {
        IDictionary<TKey, TValue> Items { get; set; }
        void Initialize(int refreshIntervalMins);
        void AddOrUpdateItem(TKey key, TValue value);

        IEnumerable<KeyValuePair<TKey, TValue>> GetValidItems();
        IEnumerable<KeyValuePair<TKey, TValue>> GetExpiredItems();

    }
}