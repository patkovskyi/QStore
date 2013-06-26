namespace QStore.Strings.Interfaces
{
    using System.Collections.Generic;

    using QStore.Core.Interfaces;

    public interface IStringMap<TValue> : ISequenceMap<char, TValue>
    {
        new KeyValuePair<string, TValue> GetByIndex(long index);

        new IEnumerable<KeyValuePair<string, TValue>> GetByPrefixWithValue(IEnumerable<char> prefix);

        new string GetKeyByIndex(long index);
    }
}