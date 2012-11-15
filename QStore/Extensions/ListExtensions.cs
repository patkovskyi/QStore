namespace QStore.Extensions
{
    using System.Collections.Generic;

    internal static class ListExtensions
    {
        #region Methods

        internal static int SortedInsert<T>(this List<T> list, T item, IComparer<T> comparer)
        {
            var iList = (IList<T>)list;
            return IListExtensions.SortedInsert(ref iList, item, comparer);
        }

        #endregion
    }
}