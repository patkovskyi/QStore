namespace QStore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class QStringSet : IStringSet, IEnumerable<string>
    {
        private QSet<char> qSet;

        private QStringSet()
        {
        }

        public static QStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return new QStringSet() { qSet = QSet<char>.Create(strings, comparer) };
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            throw new NotImplementedException();
        }

        string IStringSet.GetByIndex(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerable<char> ISequenceSet<char>.GetByIndex(int index)
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
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int GetIndex(IEnumerable<char> sequence)
        {
            throw new NotImplementedException();
        }
    }
}