namespace QStore.Strings
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using QStore.Core;

    [DataContract]
    [Serializable]
    public class QStringSet
    {
        [DataMember(Order = 1)]
        public QSet Set { get; protected set; }

        public int Count
        {
            get
            {
                return this.Set.Count;
            }
        }

        public IComparer<char> Comparer
        {
            get
            {
                return this.Set.Comparer;
            }
        }

        public static QStringSet Create(IEnumerable<string> strings, IComparer<char> comparer)
        {
            return new QStringSet { Set = QSet.Create(strings, comparer) };
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            return this.Set.Contains(sequence);
        }

        public IEnumerable<string> Enumerate()
        {
            return this.Set.Enumerate().Wrap();
        }

        public IEnumerable<string> EnumerateByPrefix(IEnumerable<char> prefix)
        {
            return this.Set.EnumerateByPrefix(prefix).Wrap();
        }

        public void SetComparer(IComparer<char> comparer)
        {
            this.Set.SetComparer(comparer);
        }
    }
}