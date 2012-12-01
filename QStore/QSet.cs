namespace QStore
{
    using System.Collections;
    using System.Collections.Generic;

    public class QSet<T> : ISequenceSet<T>
    {
        public bool Contains(IEnumerable<T> sequence)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetByPrefix(IEnumerable<T> prefix)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int GetIndex(IEnumerable<T> sequence)
        {
            throw new System.NotImplementedException();
        }
    }
}