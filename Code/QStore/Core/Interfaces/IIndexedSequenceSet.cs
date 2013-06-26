namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface IIndexedSequenceSet<T> : ISequenceSet<T>
    {
        List<T> GetByIndex(long index);

        long GetIndex(IEnumerable<T> sequence);

        IEnumerable<KeyValuePair<T[], long>> GetByPrefixWithIndex(IEnumerable<T> prefix);
    }
}