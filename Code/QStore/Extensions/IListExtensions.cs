namespace QStore.Extensions
{
    using System.Collections.Generic;

    using QStore.Structs;

    internal static class IListExtensions
    {
        internal static int GetTransitionIndex(
            this IList<QTransition> transitions,
            char symbol,
            IComparer<char> comparer,
            int lower,
            int upper)
        {
            const int BinarySearchThreshold = 1;
            if (upper - lower >= BinarySearchThreshold)
            {
                // binary search
                upper--;
                while (lower <= upper)
                {
                    int middle = lower + ((upper - lower) >> 1);
                    int comparisonResult = comparer.Compare(transitions[middle].Symbol, symbol);
                    if (comparisonResult == 0)
                    {
                        return middle;
                    }

                    if (comparisonResult > 0)
                    {
                        upper = middle - 1;
                    }
                    else
                    {
                        lower = middle + 1;
                    }
                }

                return ~lower;
            }

            // linear search
            int i;
            for (i = lower; i < upper; i++)
            {
                int comp = comparer.Compare(transitions[i].Symbol, symbol);
                if (comp == 0)
                {
                    return i;
                }

                if (comp > 0)
                {
                    return ~i;
                }
            }

            return ~i;
        }

        internal static int GetUpperBound<T>(this IList<T> list, IList<int> lowerBounds, int lowerIndexIndex)
        {
            return lowerIndexIndex + 1 < lowerBounds.Count ? lowerBounds[lowerIndexIndex + 1] : list.Count;
        }
    }
}