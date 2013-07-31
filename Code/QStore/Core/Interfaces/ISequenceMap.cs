namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface ISequenceMap<TKey, TValue> : IIndexedSequenceSet<TKey>
    {
        TValue[] Values { get; }

        TValue this[IEnumerable<TKey> key] { get; set; }

        IEnumerable<KeyValuePair<TKey[], TValue>> EnumerateByPrefixWithValue(IEnumerable<TKey> prefix);

        IEnumerable<KeyValuePair<TKey[], TValue>> EnumerateWithValue();

        KeyValuePair<TKey[], TValue> GetByIndexWithValue(int index);

        TKey[] GetKeyByIndex(int index);

        bool TryGetValue(IEnumerable<TKey> key, out TValue value);
    }
}