namespace QStore.Strings
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using QStore.Core;

    [DataContract]
    [Serializable]
    public class QIndexedStringSet
    {
        [DataMember(Order = 1)]
        public QIndexedSet IndexedSet { get; protected set; }

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

        public static QIndexedStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return new QIndexedStringSet { IndexedSet = QIndexedSet.Create(strings, comparer) };
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            return this.IndexedSet.Contains(sequence);
        }

        public IEnumerable<string> Enumerate()
        {
            return this.IndexedSet.Enumerate().Wrap();
        }

        public IEnumerable<string> EnumerateByPrefix(IEnumerable<char> prefix)
        {
            return this.IndexedSet.EnumerateByPrefix(prefix).Wrap();
        }

        public IEnumerable<KeyValuePair<string, int>> EnumerateByPrefixWithIndex(IEnumerable<char> prefix)
        {
            return this.IndexedSet.EnumerateByPrefixWithIndex(prefix).Wrap();
        }

        public IEnumerable<KeyValuePair<string, int>> EnumerateWithIndex()
        {
            return this.IndexedSet.EnumerateWithIndex().Wrap();
        }

        public string GetByIndex(int index)
        {
            return this.IndexedSet.GetByIndex(index).Wrap();
        }

        public int GetIndex(IEnumerable<char> sequence)
        {
            return this.IndexedSet.GetIndex(sequence);
        }

        public void SetComparer(IComparer<char> comparer)
        {
            this.IndexedSet.SetComparer(comparer);
        }
    }
}