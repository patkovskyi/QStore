namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface IIndexedSequenceSet<T> : ISequenceSet<T>
    {
        IEnumerable<KeyValuePair<T[], int>> EnumerateByPrefixWithIndex(IEnumerable<T> prefix);

        IEnumerable<KeyValuePair<T[], int>> EnumerateWithIndex();

        T[] GetByIndex(int index);

        int GetIndex(IEnumerable<T> sequence);
    }
}