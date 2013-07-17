namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface IIndexedSequenceSet<T> : ISequenceSet<T>
    {
        T[] GetByIndex(int index);

        IEnumerable<KeyValuePair<T[], int>> GetByPrefixWithIndex(IEnumerable<T> prefix);

        int GetIndex(IEnumerable<T> sequence);

        IEnumerable<KeyValuePair<T[], int>> GetWithIndex();
    }
}