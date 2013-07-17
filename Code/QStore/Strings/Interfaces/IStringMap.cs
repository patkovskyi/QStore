namespace QStore.Strings.Interfaces
{
    using System.Collections.Generic;

    using QStore.Core.Interfaces;

    public interface IStringMap<TValue> : ISequenceMap<char, TValue>
    {
        new KeyValuePair<string, TValue> GetByIndex(int index);

        new IEnumerable<KeyValuePair<string, TValue>> GetByPrefixWithValue(IEnumerable<char> prefix);

        new string GetKeyByIndex(int index);

        new IEnumerable<KeyValuePair<string, TValue>> GetWithValue();
    }
}