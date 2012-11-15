namespace QStore
{
    using System.Collections;
    using System.Collections.Generic;

    public class QSetCore<T> : IEnumerable<IEnumerable<T>>
    {
        private IList<SequenceSetTransition> transitions;

        private T[] alphabet;

        private IList<int> lowerTransitionIndexes;

        private int start;

        private IComparer<T> symbolComparer;

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
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
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}