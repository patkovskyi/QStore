namespace QStore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class QSet<T> : ISequenceSet<T>, IEnumerable<IEnumerable<T>>
    {
        public static QSet<T> Create(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IEnumerable<T> sequence)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetByIndex(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IEnumerable<T>> GetByPrefix(IEnumerable<T> prefix)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        /// TODO: check if "int" is enough
        public int GetIndex(IEnumerable<T> sequence)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}