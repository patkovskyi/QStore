namespace QStore.Core.Extensions
{
    using System.Collections.Generic;

    internal static class IListExtensions
    {
        internal static int GetUpperIndex<T>(this IList<T> list, IList<int> lowerIndexes, int lowerIndexIndex)
        {
            return lowerIndexIndex + 1 < lowerIndexes.Count ? lowerIndexes[lowerIndexIndex + 1] : list.Count;
        }
    }
}