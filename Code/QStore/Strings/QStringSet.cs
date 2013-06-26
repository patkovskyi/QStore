namespace QStore.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Core;
    using QStore.Strings.Interfaces;

    public class QStringSet : QSet<char>, IStringSet
    {
        public static QStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return QSet<char>.Create<QStringSet>(strings, comparer);
        }

        public new IEnumerable<string> GetByPrefix(IEnumerable<char> prefix)
        {
            return base.GetByPrefix(prefix).Select(s => new string(s.ToArray()));
        }

        public new IEnumerator<string> GetEnumerator()
        {
            return this.Enumerate(this.RootState).Select(s => new string(s.ToArray())).GetEnumerator();
        }
    }
}