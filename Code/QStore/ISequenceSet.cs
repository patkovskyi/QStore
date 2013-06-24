namespace QStore
{
    using System.Collections.Generic;

    public interface ISequenceSet<T>
    {
        bool Contains(IEnumerable<T> sequence);

        IEnumerable<IEnumerable<T>> GetByPrefix(IEnumerable<T> prefix);
    }
}