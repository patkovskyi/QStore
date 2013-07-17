namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface ISequenceMap<TKey, TValue> : IIndexedSequenceSet<TKey>
    {
        TValue this[IEnumerable<TKey> key] { get; set; }

        new KeyValuePair<TKey[], TValue> GetByIndex(int index);

        IEnumerable<KeyValuePair<TKey[], TValue>> GetByPrefixWithValue(IEnumerable<TKey> prefix);

        TKey[] GetKeyByIndex(int index);

        TValue GetValueByIndex(int index);

        IEnumerable<KeyValuePair<TKey[], TValue>> GetWithValue();

        void SetValueByIndex(int index, TValue value);

        bool TryGetValue(IEnumerable<TKey> key, out TValue value);
    }
}