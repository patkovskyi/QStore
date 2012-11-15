namespace QStore.Comparers
{
    using System.Collections.Generic;

    internal class SequenceKeyValueComparer<TKey, TValue> : Comparer<KeyValuePair<IEnumerable<TKey>, TValue>>
    {
        #region Fields

        private readonly IComparer<IEnumerable<TKey>> keyComparer;

        private readonly IComparer<TValue> valueComparer;

        #endregion

        #region Constructors and Destructors

        public SequenceKeyValueComparer(IComparer<TKey> keyElementComparer, IComparer<TValue> valueComparer)
        {
            this.keyComparer = new SequenceComparer<TKey>(keyElementComparer);
            this.valueComparer = valueComparer;
        }

        #endregion

        #region Public Methods and Operators

        public override int Compare(KeyValuePair<IEnumerable<TKey>, TValue> x, KeyValuePair<IEnumerable<TKey>, TValue> y)
        {
            int comp = this.keyComparer.Compare(x.Key, y.Key);
            if (comp == 0) comp = this.valueComparer.Compare(x.Value, y.Value);
            return comp;
        }

        #endregion
    }
}