namespace QStore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class QStringSet : IStringSet, IEnumerable<string>
    {
        public static QStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetByPrefix(IEnumerable<char> prefix)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int GetIndex(IEnumerable<char> sequence)
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

        IEnumerable<IEnumerable<char>> ISequenceSet<char>.GetByPrefix(IEnumerable<char> prefix)
        {
            return this.GetByPrefix(prefix);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}