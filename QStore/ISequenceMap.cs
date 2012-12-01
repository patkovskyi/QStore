namespace QStore
{
    using System.Collections.Generic;

    public interface ISequenceMap<TKey, TValue> : IEnumerable<KeyValuePair<IEnumerable<TKey>, TValue>>
    {
        bool TryGetValue(IEnumerable<TKey> key, out TValue value);
    }
}