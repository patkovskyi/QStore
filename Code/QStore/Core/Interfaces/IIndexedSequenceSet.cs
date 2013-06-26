namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface IIndexedSequenceSet<T> : ISequenceSet<T>
    {
        List<T> GetByIndex(long index);

        IEnumerable<KeyValuePair<T[], long>> GetByPrefixWithIndex(IEnumerable<T> prefix);

        long GetIndex(IEnumerable<T> sequence);
    }
}