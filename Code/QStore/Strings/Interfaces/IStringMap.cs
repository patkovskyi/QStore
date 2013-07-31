namespace QStore.Strings.Interfaces
{
    using System.Collections.Generic;

    public interface IStringMap<TValue> : IIndexedStringSet
    {
        TValue[] Values { get; }

        TValue this[IEnumerable<char> key] { get; set; }

        IEnumerable<KeyValuePair<string, TValue>> EnumerateByPrefixWithValue(IEnumerable<char> prefix);

        IEnumerable<KeyValuePair<string, TValue>> EnumerateWithValue();

        KeyValuePair<string, TValue> GetByIndexWithValue(int index);

        string GetKeyByIndex(int index);

        bool TryGetValue(IEnumerable<char> key, out TValue value);
    }
}