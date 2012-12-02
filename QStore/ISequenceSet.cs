namespace QStore
{
    using System.Collections.Generic;

    public interface ISequenceSet<T>
    {
        bool Contains(IEnumerable<T> sequence);

        IEnumerable<T> GetByIndex(int index);

        IEnumerable<IEnumerable<T>> GetByPrefix(IEnumerable<T> prefix);

        /// TODO: check if "int" is enough
        int GetIndex(IEnumerable<T> sequence);
    }
}