namespace QStore.Strings
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using QStore.Core;
    using QStore.Strings.Interfaces;

    [DataContract]
    [Serializable]
    public class QStringMap<TValue> : IStringMap<TValue>
    {
        [DataMember(Order = 1)]
        public QMap<char, TValue> Map { get; protected set; }

        public int Count
        {
            get
            {
                return this.Map.Count;
            }
        }

        public IComparer<char> Comparer
        {
            get
            {
                return this.Map.Comparer;
            }
        }

        public TValue[] Values
        {
            get
            {
                return this.Map.Values;
            }
        }

        public TValue this[IEnumerable<char> key]
        {
            get
            {
                return this.Map[key];
            }

            set
            {
                this.Map[key] = value;
            }
        }

        public static QStringMap<TValue> Create(IEnumerable<string> sequences, IComparer<char> comparer)
        {
            return new QStringMap<TValue> { Map = QMap<char, TValue>.Create(sequences, comparer) };
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            return this.Map.Contains(sequence);
        }

        public IEnumerable<string> Enumerate()
        {
            return this.Map.Enumerate().Wrap();
        }

        public IEnumerable<string> EnumerateByPrefix(IEnumerable<char> prefix)
        {
            return this.Map.EnumerateByPrefix(prefix).Wrap();
        }

        public IEnumerable<KeyValuePair<string, int>> EnumerateByPrefixWithIndex(IEnumerable<char> prefix)
        {
            return this.Map.EnumerateByPrefixWithIndex(prefix).Wrap();
        }

        public IEnumerable<KeyValuePair<string, TValue>> EnumerateByPrefixWithValue(IEnumerable<char> prefix)
        {
            return this.Map.EnumerateByPrefixWithValue(prefix).Wrap();
        }

        public IEnumerable<KeyValuePair<string, int>> EnumerateWithIndex()
        {
            return this.Map.EnumerateWithIndex().Wrap();
        }

        public IEnumerable<KeyValuePair<string, TValue>> EnumerateWithValue()
        {
            return this.Map.EnumerateWithValue().Wrap();
        }

        public string GetByIndex(int index)
        {
            return this.Map.GetByIndex(index).Wrap();
        }

        public KeyValuePair<string, TValue> GetByIndexWithValue(int index)
        {
            return this.Map.GetByIndexWithValue(index).Wrap();
        }

        public int GetIndex(IEnumerable<char> sequence)
        {
            return this.Map.GetIndex(sequence);
        }

        public string GetKeyByIndex(int index)
        {
            return this.Map.GetKeyByIndex(index).Wrap();
        }

        public void SetComparer(IComparer<char> comparer)
        {
            this.Map.SetComparer(comparer);
        }

        public bool TryGetValue(IEnumerable<char> key, out TValue value)
        {
            return this.Map.TryGetValue(key, out value);
        }
    }
}