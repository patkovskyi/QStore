namespace QStore.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Core.Interfaces;

    public class QMap<TKey, TValue> : QIndexedSet<TKey>, 
                                      ISequenceMap<TKey, TValue>, 
                                      IEnumerable<KeyValuePair<TKey[], TValue>>
    {
        protected internal TValue[] Values;

        public TValue this[IEnumerable<TKey> sequence]
        {
            get
            {
                TValue value;
                if (this.TryGetValue(sequence, out value))
                {
                    return value;
                }

                throw new KeyNotFoundException();
            }
        }

        public new KeyValuePair<TKey[], TValue> GetByIndex(long index)
        {
            return new KeyValuePair<TKey[], TValue>(base.GetByIndex(index).ToArray(), this.Values[index]);
        }

        public IEnumerable<KeyValuePair<TKey[], TValue>> GetByPrefixWithValue(IEnumerable<TKey> prefix)
        {
            return
                this.GetByPrefixWithIndex(prefix)
                    .Select(p => new KeyValuePair<TKey[], TValue>(p.Key, this.Values[p.Value]));
        }

        public new IEnumerator<KeyValuePair<TKey[], TValue>> GetEnumerator()
        {
            return
                this.Enumerate(this.RootState)
                    .Select((key, i) => new KeyValuePair<TKey[], TValue>(key, this.Values[i]))
                    .GetEnumerator();
        }

        public List<TKey> GetKeyByIndex(long index)
        {
            return base.GetByIndex(index);
        }

        public TValue GetValueByIndex(long index)
        {
            return this.Values[index];
        }

        public bool TryGetValue(IEnumerable<TKey> key, out TValue value)
        {
            long index = this.GetIndex(key);
            if (index < 0)
            {
                value = default(TValue);
                return false;
            }

            value = this.Values[index];
            return true;
        }
    }
}