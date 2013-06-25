namespace QStore
{
    using System.Collections.Generic;
    using System.Linq;

    public class QIndexedStringSet : QIndexedSet<char>, 
                                     IIndexedStringSet, 
                                     IEnumerable<string>, 
                                     IEnumerable<KeyValuePair<string, long>>
    {
        public static QIndexedStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return QIndexedSet<char>.Create<QIndexedStringSet>(strings, comparer);
        }

        public new string GetByIndex(long index)
        {
            return new string(base.GetByIndex(index).ToArray());
        }

        IEnumerator<KeyValuePair<string, long>> IEnumerable<KeyValuePair<string, long>>.GetEnumerator()
        {
            return
                this.EnumerateStrings().Select(s => new KeyValuePair<string, long>(s, this.GetIndex(s))).GetEnumerator();
        }

        public new IEnumerator<string> GetEnumerator()
        {
            return this.EnumerateStrings().GetEnumerator();
        }

        protected IEnumerable<string> EnumerateStrings()
        {
            return this.Enumerate(this.RootState).Select(s => new string(s.ToArray()));
        }
    }
}