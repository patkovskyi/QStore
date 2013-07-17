namespace QStore.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Core;
    using QStore.Strings.Interfaces;

    public class QIndexedStringSet : QIndexedSet<char>, IIndexedStringSet
    {
        public static QIndexedStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return QIndexedSet<char>.Create<QIndexedStringSet>(strings, comparer);
        }

        public new string GetByIndex(int index)
        {
            return new string(base.GetByIndex(index));
        }

        public new IEnumerable<KeyValuePair<string, int>> GetByPrefixWithIndex(IEnumerable<char> prefix)
        {
            return
                base.GetByPrefixWithIndex(prefix).Select(p => new KeyValuePair<string, int>(new string(p.Key), p.Value));
        }

        public new IEnumerator<string> GetEnumerator()
        {
            return this.Enumerate(this.RootTransition).Select(s => new string(s)).GetEnumerator();
        }

        public new IEnumerable<KeyValuePair<string, int>> GetWithIndex()
        {
            return this.Enumerate(this.RootTransition).Select((s, i) => new KeyValuePair<string, int>(new string(s), i));
        }
    }
}