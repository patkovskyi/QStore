using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Spellchecker;

namespace QSpell.Extensions
{
    internal static class ListExtensions
    {
        internal static int SortedInsert<T>(this List<T> list, T item, IComparer<T> comparer)
        {
            var iList = (IList<T>)list;
            return IListExtensions.SortedInsert(ref iList, item, comparer);
        }
    }
}
