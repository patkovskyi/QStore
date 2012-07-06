using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Spellchecker
{
    public static class SortedSequence<T>
    {
        public static IEnumerable<T> Join(IComparer<T> comparer, params IEnumerable<T>[] sequences)
        {
            return Join(sequences, comparer);
        }

        public static IEnumerable<T> Join(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer)
        {
            var enumerators = sequences.Select(s => s.GetEnumerator()).ToArray();
            var hasElements = enumerators.Select(s => s.MoveNext()).ToArray();
            int minIndex;
            while ((minIndex = Array.IndexOf(hasElements, true)) >= 0)
            {
                for (int i = 0; i < enumerators.Length; i++)
                {
                    if (hasElements[i])
                    {
                        if (comparer.Compare(enumerators[i].Current, enumerators[minIndex].Current) < 0)
                        {
                            minIndex = i;
                        }
                    }
                }
                yield return enumerators[minIndex].Current;
                hasElements[minIndex] = enumerators[minIndex].MoveNext();
            }
        }
    }
}
