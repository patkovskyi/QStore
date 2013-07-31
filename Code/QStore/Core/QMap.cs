namespace QStore.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using QStore.Core.Interfaces;

    [DataContract]
    [Serializable]
    public class QMap<TKey, TValue> : ISequenceMap<TKey, TValue>
    {
        [DataMember(Order = 1)]
        protected internal QIndexedSet<TKey> IndexedSet;

        [DataMember(Order = 2)]
        public TValue[] Values { get; protected set; }

        public int Count
        {
            get
            {
                return this.IndexedSet.Count;
            }
        }

        public IComparer<TKey> Comparer
        {
            get
            {
                return this.IndexedSet.Comparer;
            }
        }

        public TValue this[IEnumerable<TKey> sequence]
        {
            get
            {
                int index = this.GetIndex(sequence);
                if (index < 0)
                {
                    throw new KeyNotFoundException();
                }

                return this.Values[index];
            }

            set
            {
                int index = this.GetIndex(sequence);
                if (index < 0)
                {
                    throw new KeyNotFoundException();
                }

                this.Values[index] = value;
            }
        }

        public static QMap<TKey, TValue> Create(IEnumerable<IEnumerable<TKey>> keySequences, IComparer<TKey> comparer)
        {
            if (keySequences == null)
            {
                throw new ArgumentNullException("keySequences");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            var indexedSet = QIndexedSet<TKey>.Create(keySequences, comparer);
            return new QMap<TKey, TValue> { IndexedSet = indexedSet, Values = new TValue[indexedSet.Count] };
        }

        public bool Contains(IEnumerable<TKey> sequence)
        {
            return this.IndexedSet.Contains(sequence);
        }

        public IEnumerable<TKey[]> Enumerate()
        {
            return this.IndexedSet.Enumerate();
        }

        public IEnumerable<TKey[]> EnumerateByPrefix(IEnumerable<TKey> prefix)
        {
            return this.IndexedSet.EnumerateByPrefix(prefix);
        }

        public IEnumerable<KeyValuePair<TKey[], int>> EnumerateByPrefixWithIndex(IEnumerable<TKey> prefix)
        {
            return this.IndexedSet.EnumerateByPrefixWithIndex(prefix);
        }

        public IEnumerable<KeyValuePair<TKey[], TValue>> EnumerateByPrefixWithValue(IEnumerable<TKey> prefix)
        {
            return
                this.EnumerateByPrefixWithIndex(prefix)
                    .Select(p => new KeyValuePair<TKey[], TValue>(p.Key, this.Values[p.Value]));
        }

        public IEnumerable<KeyValuePair<TKey[], int>> EnumerateWithIndex()
        {
            return this.IndexedSet.EnumerateWithIndex();
        }

        public IEnumerable<KeyValuePair<TKey[], TValue>> EnumerateWithValue()
        {
            return this.Enumerate().Select((key, i) => new KeyValuePair<TKey[], TValue>(key, this.Values[i]));
        }

        public TKey[] GetByIndex(int index)
        {
            return this.IndexedSet.GetByIndex(index);
        }

        public KeyValuePair<TKey[], TValue> GetByIndexWithValue(int index)
        {
            return new KeyValuePair<TKey[], TValue>(this.IndexedSet.GetByIndex(index), this.Values[index]);
        }

        public int GetIndex(IEnumerable<TKey> sequence)
        {
            return this.IndexedSet.GetIndex(sequence);
        }

        public TKey[] GetKeyByIndex(int index)
        {
            return this.IndexedSet.GetByIndex(index);
        }

        public void SetComparer(IComparer<TKey> comparer)
        {
            this.IndexedSet.SetComparer(comparer);
        }

        public bool TryGetValue(IEnumerable<TKey> key, out TValue value)
        {
            int index = this.GetIndex(key);
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