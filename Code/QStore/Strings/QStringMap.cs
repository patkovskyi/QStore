namespace QStore.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Core;
    using QStore.Strings.Interfaces;

    public class QStringMap<TValue> : QMap<char, TValue>, IStringMap<TValue>
    {
        public static QStringMap<TValue> Create(IEnumerable<string> sequences, IComparer<char> comparer)
        {
            var map = QIndexedSet<char>.Create<QStringMap<TValue>>(sequences, comparer);
            map.Values = new TValue[map.Count];
            return map;
        }

        public new KeyValuePair<string, TValue> GetByIndex(int index)
        {
            var basePair = base.GetByIndex(index);
            return new KeyValuePair<string, TValue>(new string(basePair.Key), this.Values[index]);
        }

        public new IEnumerable<KeyValuePair<string, TValue>> GetByPrefixWithValue(IEnumerable<char> prefix)
        {
            return
                this.GetByPrefixWithIndex(prefix)
                    .Select(p => new KeyValuePair<string, TValue>(new string(p.Key), this.Values[p.Value]));
        }

        public new IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return
                this.Enumerate(this.RootTransition)
                    .Select((key, i) => new KeyValuePair<string, TValue>(new string(key), this.Values[i]))
                    .GetEnumerator();
        }

        public new string GetKeyByIndex(int index)
        {
            return new string(base.GetKeyByIndex(index));
        }

        public new IEnumerable<KeyValuePair<string, TValue>> GetWithValue()
        {
            return
                this.Enumerate(this.RootTransition)
                    .Select((s, i) => new KeyValuePair<string, TValue>(new string(s), this.Values[i]));
        }
    }
}