namespace QStore
{
    using System.Collections.Generic;

    public interface IIndexedSequenceSet<T> : ISequenceSet<T>
    {
        IEnumerable<T> GetByIndex(int index);

        long GetIndex(IEnumerable<T> sequence);
    }
}