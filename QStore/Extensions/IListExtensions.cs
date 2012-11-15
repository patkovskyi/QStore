namespace QStore.Extensions
{
    using System;
    using System.Collections.Generic;

    internal static class IListExtensions
    {
        #region Constants

        internal const int BinarySearchThreshold = 10;

        #endregion

        #region Methods

        internal static int BinarySearch<T, S>(this IList<T> list, S item, Func<T, S> map, IComparer<S> comparer, int lower, int upper)
        {
            if (object.ReferenceEquals(null, list)) throw new ArgumentNullException("list");
            if (object.ReferenceEquals(null, comparer)) throw new ArgumentNullException("comparer");
            upper--;
            while (lower <= upper)
            {
                int middle = (lower + upper) / 2;
                int comparisonResult = comparer.Compare(map(list[middle]), item);
                if (comparisonResult == 0) return middle;
                else if (comparisonResult > 0) upper = middle - 1;
                else lower = middle + 1;
            }

            return ~lower;
        }

        internal static int BinarySearch<T>(this IList<T> list, T item, IComparer<T> comparer, int lower, int upper)
        {
            if (object.ReferenceEquals(null, list)) throw new ArgumentNullException("list");
            if (object.ReferenceEquals(null, comparer)) throw new ArgumentNullException("comparer");
            upper--;
            while (lower <= upper)
            {
                int middle = (lower + upper) / 2;
                int comparisonResult = comparer.Compare(list[middle], item);
                if (comparisonResult == 0) return middle;
                else if (comparisonResult > 0) upper = middle - 1;
                else lower = middle + 1;
            }

            return ~lower;
        }

        internal static int BinarySearch<T>(this IList<T> list, T item, IComparer<T> comparer)
        {
            return BinarySearch(list, item, comparer, 0, list.Count);
        }

        internal static int BinarySearch<T>(this IList<T> list, T item)
        {
            return BinarySearch(list, item, Comparer<T>.Default);
        }

        internal static int GetUpperIndex<T>(this IList<T> list, IList<int> lowerIndexes, int lowerIndexIndex)
        {
            return lowerIndexIndex + 1 < lowerIndexes.Count ? lowerIndexes[lowerIndexIndex + 1] : list.Count;
        }

        internal static void InsertAt<T>(ref IList<T> list, T item, int index)
        {
            if (list is T[])
            {
                var tmp = (T[])list;
                ArrayExtensions.Insert(ref tmp, item, index);
                list = tmp;
            }
            else list.Insert(index, item);
        }

        internal static int LinearSearch<T, S>(this IList<T> list, S item, Func<T, S> map, IComparer<S> comparer, int lower, int upper)
        {
            if (object.ReferenceEquals(null, list)) throw new ArgumentNullException("list");
            if (object.ReferenceEquals(null, comparer)) throw new ArgumentNullException("comparer");

            for (int i = lower; i < upper; i++)
            {
                int comp = comparer.Compare(map(list[i]), item);
                if (comp == 0) return i;
                else if (comp > 0) return ~i;
            }

            return ~upper;
        }

        internal static int LinearSearch<T>(this IList<T> list, T item, IComparer<T> comparer, int lower, int upper)
        {
            if (object.ReferenceEquals(null, list)) throw new ArgumentNullException("list");
            if (object.ReferenceEquals(null, comparer)) throw new ArgumentNullException("comparer");

            for (int i = lower; i < upper; i++)
            {
                int comp = comparer.Compare(list[i], item);
                if (comp == 0) return i;
                else if (comp > 0) return ~i;
            }

            return ~upper;
        }

        internal static int LinearSearch<T>(this IList<T> list, T item, IComparer<T> comparer)
        {
            return LinearSearch(list, item, comparer, 0, list.Count - 1);
        }

        internal static int LinearSearch<T>(this IList<T> list, T item)
        {
            return LinearSearch(list, item, Comparer<T>.Default);
        }

        internal static int OptimalSearch<T, S>(this IList<T> list, S item, Func<T, S> map, IComparer<S> comparer, int lower, int upper)
        {
            if (upper - lower > BinarySearchThreshold) return BinarySearch(list, item, map, comparer, lower, upper);
            else return LinearSearch(list, item, map, comparer, lower, upper);
        }

        internal static int OptimalSearch<T, S>(this IList<T> list, S item, Func<T, S> map, IComparer<S> comparer)
        {
            return OptimalSearch(list, item, map, comparer, 0, list.Count);
        }

        internal static int OptimalSearch<T>(this IList<T> list, T item, IComparer<T> comparer, int lower, int upper)
        {
            if (upper - lower > BinarySearchThreshold) return BinarySearch(list, item, comparer, lower, upper);
            else return LinearSearch(list, item, comparer, lower, upper);
        }

        internal static int OptimalSearch<T>(this IList<T> list, T item, IComparer<T> comparer)
        {
            return OptimalSearch(list, item, comparer, 0, list.Count);
        }

        internal static int OptimalSearch<T>(this IList<T> list, T item)
        {
            return OptimalSearch(list, item, Comparer<T>.Default);
        }

        internal static void RemoveAt<T>(ref IList<T> list, int index)
        {
            if (list is T[])
            {
                var arr = list as T[];
                Array.Copy(arr, index + 1, arr, index, list.Count - index - 1);
                Array.Resize(ref arr, list.Count - 1);
                list = arr;
            }
            else list.RemoveAt(index);
        }

        // TODO: test this well.
        internal static void RemoveRange<T>(ref IList<T> list, int index, int count)
        {
            if (list is List<T>) (list as List<T>).RemoveRange(index, count);
            else if (list is T[])
            {
                var arr = list as T[];
                Array.Copy(arr, index + count, arr, index, list.Count - index - count);
                Array.Resize(ref arr, list.Count - count);
                list = arr;
            }
            else
            {
                for (int i = index + count; i-- > index;)
                {
                    RemoveAt(ref list, i);
                }
            }
        }

        internal static int SortedInsert<T>(ref IList<T> list, T item, IComparer<T> comparer, int lower, int upper)
        {
            int index = OptimalSearch(list, item, comparer, lower, upper);
            if (index < 0) index = ~index;
            InsertAt(ref list, item, index);
            return index;
        }

        internal static int SortedInsert<T>(ref IList<T> list, T item, IComparer<T> comparer)
        {
            return SortedInsert(ref list, item, comparer, 0, list.Count);
        }

        internal static int SortedInsert<T>(ref IList<T> list, T item)
        {
            return SortedInsert(ref list, item, Comparer<T>.Default);
        }

        #endregion

        // TODO: test this well.
    }
}