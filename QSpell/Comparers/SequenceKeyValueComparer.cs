using System.Collections.Generic;

namespace QSpell.Comparers
{
    internal class SequenceKeyValueComparer<K, V> : Comparer<KeyValuePair<IEnumerable<K>, V>>
    {
        private IComparer<IEnumerable<K>> keyComparer;
        private IComparer<V> valueComparer;

        public override int Compare(KeyValuePair<IEnumerable<K>, V> x, KeyValuePair<IEnumerable<K>, V> y)
        {
            int comp = keyComparer.Compare(x.Key, y.Key);
            if (comp == 0)
            {
                comp = valueComparer.Compare(x.Value, y.Value);
            }
            return comp;
        }

        public SequenceKeyValueComparer(IComparer<K> keyElementComparer, IComparer<V> valueComparer)
        {
            this.keyComparer = new SequenceComparer<K>(keyElementComparer);
            this.valueComparer = valueComparer;
        }
    }
}
