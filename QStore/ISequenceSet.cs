namespace QStore
{
    using System.Collections.Generic;

    public interface ISequenceSet<T> : IEnumerable<IEnumerable<T>>
    {
        bool Contains(IEnumerable<T> sequence);

        IEnumerable<T> GetByPrefix(IEnumerable<T> prefix);

        int GetIndex(IEnumerable<T> sequence);
    }
}