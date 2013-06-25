namespace QStore
{
    using System.Collections.Generic;

    public interface IIndexedSequenceSet<T> : ISequenceSet<T>
    {
        List<T> GetByIndex(long index);

        long GetIndex(IEnumerable<T> sequence);
    }
}