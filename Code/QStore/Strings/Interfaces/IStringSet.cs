namespace QStore.Strings.Interfaces
{
    using System.Collections.Generic;

    public interface IStringSet
    {
        int Count { get; }

        IComparer<char> Comparer { get; }

        bool Contains(IEnumerable<char> sequence);

        IEnumerable<string> Enumerate();

        IEnumerable<string> EnumerateByPrefix(IEnumerable<char> prefix);

        /// <summary>
        /// Call this only after deserialization. Setting wrong comparer will make your structure act wrong.        
        /// </summary>
        /// <param name="comparer">Comparer used when structure was initially created.</param>
        void SetComparer(IComparer<char> comparer);
    }
}