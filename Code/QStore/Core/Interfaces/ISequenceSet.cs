namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface ISequenceSet<T>
    {
        bool Contains(IEnumerable<T> sequence);

        IEnumerable<T[]> GetByPrefix(IEnumerable<T> prefix);
    }
}