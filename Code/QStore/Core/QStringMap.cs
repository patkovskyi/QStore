namespace QStore.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class QStringMap<TValue>
    {
        [DataMember(Order = 1)]
        protected internal QIndexedStringSet StringIndexedSet;

        [DataMember(Order = 2)]
        public TValue[] Values { get; protected set; }

        public int Count
        {
            get
            {
                return this.StringIndexedSet.Count;
            }
        }

        public IComparer<char> Comparer
        {
            get
            {
                return this.StringIndexedSet.Comparer;
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

        public static QStringMap<TValue> Create(IEnumerable<IEnumerable<char>> keySequences, IComparer<char> comparer)
        {
            if (keySequences == null)
            {
                throw new ArgumentNullException("keySequences");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            var indexedSet = QIndexedStringSet.Create(keySequences, comparer);
            return new QStringMap<TValue> { StringIndexedSet = indexedSet, Values = new TValue[indexedSet.Count] };
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            return this.StringIndexedSet.Contains(sequence);
        }

        public IEnumerable<string> Enumerate()
        {
            return this.StringIndexedSet.Enumerate();
        }

        public IEnumerable<string> EnumerateByPrefix(IEnumerable<char> prefix)
        {
            return this.StringIndexedSet.EnumerateByPrefix(prefix);
        }

        public IEnumerable<KeyValuePair<string, int>> EnumerateByPrefixWithIndex(IEnumerable<char> prefix)
        {
            return this.StringIndexedSet.EnumerateByPrefixWithIndex(prefix);
        }

        public IEnumerable<KeyValuePair<string, TValue>> EnumerateByPrefixWithValue(IEnumerable<char> prefix)
        {
            return
                this.EnumerateByPrefixWithIndex(prefix)
                    .Select(p => new KeyValuePair<string, TValue>(p.Key, this.Values[p.Value]));
        }

        public IEnumerable<KeyValuePair<string, int>> EnumerateWithIndex()
        {
            return this.StringIndexedSet.EnumerateWithIndex();
        }

        public IEnumerable<KeyValuePair<string, TValue>> EnumerateWithValue()
        {
            return this.Enumerate().Select((key, i) => new KeyValuePair<string, TValue>(key, this.Values[i]));
        }

        public string GetByIndex(int index)
        {
            return this.StringIndexedSet.GetByIndex(index);
        }

        public KeyValuePair<string, TValue> GetByIndexWithValue(int index)
        {
            return new KeyValuePair<string, TValue>(this.StringIndexedSet.GetByIndex(index), this.Values[index]);
        }

        public int GetIndex(IEnumerable<char> sequence)
        {
            return this.StringIndexedSet.GetIndex(sequence);
        }

        public string GetKeyByIndex(int index)
        {
            return this.StringIndexedSet.GetByIndex(index);
        }

        public void SetComparer(IComparer<char> comparer)
        {
            this.StringIndexedSet.SetComparer(comparer);
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