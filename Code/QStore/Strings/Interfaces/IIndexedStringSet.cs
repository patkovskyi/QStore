namespace QStore.Strings.Interfaces
{
    using System.Collections.Generic;

    public interface IIndexedStringSet : IStringSet
    {
        IEnumerable<KeyValuePair<string, int>> EnumerateByPrefixWithIndex(IEnumerable<char> prefix);

        IEnumerable<KeyValuePair<string, int>> EnumerateWithIndex();

        string GetByIndex(int index);

        int GetIndex(IEnumerable<char> sequence);
    }
}