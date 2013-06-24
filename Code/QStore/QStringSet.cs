namespace QStore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class QStringSet : IStringSet, IEnumerable<string>
    {
        private QSet<char> set;

        private QStringSet()
        {
        }

        public static QStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return new QStringSet { set = QSet<char>.Create(strings, comparer) };
        }   

        public bool Contains(IEnumerable<char> sequence)
        {
            throw new NotImplementedException();
        }             

        public IEnumerable<string> GetByPrefix(IEnumerable<char> prefix)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEnumerable<char>> ISequenceSet<char>.GetByPrefix(IEnumerable<char> prefix)
        {
            return this.GetByPrefix(prefix);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.set.Select(s => new string(s.ToArray())).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public long GetIndex(IEnumerable<char> sequence)
        {
            throw new NotImplementedException();
        }
    }
}