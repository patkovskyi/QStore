namespace QStore.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Core;
    using QStore.Strings.Interfaces;

    public class QIndexedStringSet : QIndexedSet<char>, IIndexedStringSet, IEnumerable<string>
    {
        public static QIndexedStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return QIndexedSet<char>.Create<QIndexedStringSet>(strings, comparer);
        }

        public new string GetByIndex(long index)
        {
            return new string(base.GetByIndex(index).ToArray());
        }

        public new IEnumerator<string> GetEnumerator()
        {
            return this.Enumerate(this.RootTransition).Select(s => new string(s.ToArray())).GetEnumerator();
        }
    }
}