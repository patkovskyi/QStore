namespace QStore.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Core;
    using QStore.Strings.Interfaces;

    public class QStringSet : QSet<char>, IStringSet, IEnumerable<string>
    {
        public static QStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return QSet<char>.Create<QStringSet>(strings, comparer);
        }

        public new IEnumerable<string> GetByPrefix(IEnumerable<char> prefix)
        {
            return base.GetByPrefix(prefix).Select(s => new string(s));
        }

        public new IEnumerator<string> GetEnumerator()
        {
            return this.Enumerate(this.RootTransition).Select(s => new string(s)).GetEnumerator();
        }
    }
}