namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface ISequenceMap<TKey, TValue>
    {
        bool Contains(IEnumerable<TKey> sequence);

        IEnumerable<KeyValuePair<TKey[], TValue>> GetByPrefixWithValue(IEnumerable<TKey> prefix);

        long GetIndex(IEnumerable<TKey> sequence);

        KeyValuePair<TKey[], TValue> GetByIndex(long index);

        List<TKey> GetKeyByIndex(long index);

        TValue GetValueByIndex(long index);

        bool TryGetValue(IEnumerable<TKey> key, out TValue value);
    }
}