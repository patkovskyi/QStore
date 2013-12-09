namespace QStore.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class QMap<TValue>
    {
        [DataMember(Order = 1)]
        protected internal QIndexedSet IndexedSet;

        [DataMember(Order = 2)]
        public TValue[] Values { get; protected set; }

        public int Count
        {
            get
            {
                return this.IndexedSet.Count;
            }
        }

        public IComparer<char> Comparer
        {
            get
            {
                return this.IndexedSet.Comparer;
            }
        }

        public TValue this[IEnumerable<char> sequence]
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

        public static QMap<TValue> Create(IEnumerable<IEnumerable<char>> keySequences, IComparer<char> comparer)
        {
            if (keySequences == null)
            {
                throw new ArgumentNullException("keySequences");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            var indexedSet = QIndexedSet.Create(keySequences, comparer);
            return new QMap<TValue> { IndexedSet = indexedSet, Values = new TValue[indexedSet.Count] };
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            return this.IndexedSet.Contains(sequence);
        }

        public IEnumerable<char[]> Enumerate()
        {
            return this.IndexedSet.Enumerate();
        }

        public IEnumerable<char[]> EnumerateByPrefix(IEnumerable<char> prefix)
        {
            return this.IndexedSet.EnumerateByPrefix(prefix);
        }

        public IEnumerable<KeyValuePair<char[], int>> EnumerateByPrefixWithIndex(IEnumerable<char> prefix)
        {
            return this.IndexedSet.EnumerateByPrefixWithIndex(prefix);
        }

        public IEnumerable<KeyValuePair<char[], TValue>> EnumerateByPrefixWithValue(IEnumerable<char> prefix)
        {
            return
                this.EnumerateByPrefixWithIndex(prefix)
                    .Select(p => new KeyValuePair<char[], TValue>(p.Key, this.Values[p.Value]));
        }

        public IEnumerable<KeyValuePair<char[], int>> EnumerateWithIndex()
        {
            return this.IndexedSet.EnumerateWithIndex();
        }

        public IEnumerable<KeyValuePair<char[], TValue>> EnumerateWithValue()
        {
            return this.Enumerate().Select((key, i) => new KeyValuePair<char[], TValue>(key, this.Values[i]));
        }

        public char[] GetByIndex(int index)
        {
            return this.IndexedSet.GetByIndex(index);
        }

        public KeyValuePair<char[], TValue> GetByIndexWithValue(int index)
        {
            return new KeyValuePair<char[], TValue>(this.IndexedSet.GetByIndex(index), this.Values[index]);
        }

        public int GetIndex(IEnumerable<char> sequence)
        {
            return this.IndexedSet.GetIndex(sequence);
        }

        public char[] GetKeyByIndex(int index)
        {
            return this.IndexedSet.GetByIndex(index);
        }

        public void SetComparer(IComparer<char> comparer)
        {
            this.IndexedSet.SetComparer(comparer);
        }

        public bool TryGetValue(IEnumerable<char> key, out TValue value)
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