namespace QStore.Core.Interfaces
{
    using System.Collections.Generic;

    public interface ISequenceSet<T>
    {
        int Count { get; }

        IComparer<T> Comparer { get; }

        bool Contains(IEnumerable<T> sequence);

        IEnumerable<T[]> Enumerate();

        IEnumerable<T[]> EnumerateByPrefix(IEnumerable<T> prefix);

        /// <summary>
        /// Call this only after deserialization. Setting wrong comparer will make your structure act wrong.        
        /// </summary>
        /// <param name="comparer">Comparer used when structure was initially created.</param>
        void SetComparer(IComparer<T> comparer);
    }
}