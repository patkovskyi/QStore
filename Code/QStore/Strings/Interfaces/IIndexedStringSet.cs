namespace QStore.Strings.Interfaces
{
    using System.Collections.Generic;

    using QStore.Core.Interfaces;

    public interface IIndexedStringSet : IIndexedSequenceSet<char>
    {
        new string GetByIndex(int index);

        new IEnumerable<KeyValuePair<string, int>> GetByPrefixWithIndex(IEnumerable<char> prefix);

        new IEnumerable<KeyValuePair<string, int>> GetWithIndex();
    }
}