namespace QStore
{
    using System.Collections.Generic;

    public interface ISequenceMap<TKey, TValue>
    {
        bool Contains(IEnumerable<TKey> sequence);

        IEnumerable<KeyValuePair<IEnumerable<TKey>, TValue>> GetByPrefix(IEnumerable<TKey> prefix);
        
        long GetIndex(IEnumerable<TKey> sequence);

        IEnumerable<TKey> GetKeyByIndex(int index);

        TValue GetValueByIndex(int index);

        bool TryGetValue(IEnumerable<TKey> key, out TValue value);
    }
}