namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface ISequenceMap<TKey, TValue> : IIndexedSequenceSet<TKey>
    {
        TValue this[IEnumerable<TKey> key] { get; set; }

        new KeyValuePair<TKey[], TValue> GetByIndex(long index);

        IEnumerable<KeyValuePair<TKey[], TValue>> GetByPrefixWithValue(IEnumerable<TKey> prefix);

        List<TKey> GetKeyByIndex(long index);

        TValue GetValueByIndex(long index);

        void SetValueByIndex(long index, TValue value);

        bool TryGetValue(IEnumerable<TKey> key, out TValue value);
    }
}