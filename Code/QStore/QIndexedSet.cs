namespace QStore
{
    using System;
    using System.Collections.Generic;

    public class QIndexedSet<T> : QSet<T>, IIndexedSequenceSet<T>
    {
        protected internal int[] PathsLeft;

        public static new QIndexedSet<T> Create(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer)
        {
            var indexedSet = QSet<T>.Create<QIndexedSet<T>>(sequences, comparer);
            indexedSet.PathsLeft = new int[indexedSet.Transitions.Length];

            throw new NotImplementedException();
        }

        public IEnumerable<T> GetByIndex(int index)
        {
            throw new System.NotImplementedException();
        }

        public long GetIndex(IEnumerable<T> sequence)
        {
            throw new System.NotImplementedException();
        }
    }
}