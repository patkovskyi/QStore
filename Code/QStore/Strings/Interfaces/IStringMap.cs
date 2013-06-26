namespace QStore.Strings.Interfaces
{
    using System.Collections.Generic;

    using QStore.Core.Interfaces;

    public interface IStringMap<T> : ISequenceMap<char, T>
    {
        IEnumerable<KeyValuePair<string, T>> GetByPrefixWithValue(IEnumerable<T> prefix);

        new KeyValuePair<string, T> GetByIndex(long index);

        new string GetKeyByIndex(long index);
    }
}
