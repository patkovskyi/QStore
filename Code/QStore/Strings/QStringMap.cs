namespace QStore.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Core;
    using QStore.Strings.Interfaces;

    public class QStringMap<TValue> : QMap<char, TValue>, IStringMap<TValue>, IEnumerable<KeyValuePair<string, TValue>>
    {
        public new KeyValuePair<string, TValue> GetByIndex(long index)
        {
            var basePair = base.GetByIndex(index);
            return new KeyValuePair<string, TValue>(new string(basePair.Key.ToArray()), this.Values[index]);
        }

        public new IEnumerable<KeyValuePair<string, TValue>> GetByPrefixWithValue(IEnumerable<char> prefix)
        {
            return
                base.GetByPrefixWithValue(prefix)
                    .Select(
                        basePair => new KeyValuePair<string, TValue>(new string(basePair.Key.ToArray()), basePair.Value));
        }

        public new IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return
                this.Enumerate(this.RootState)
                    .Select((key, i) => new KeyValuePair<string, TValue>(new string(key), this.Values[i]))
                    .GetEnumerator();
        }

        public new string GetKeyByIndex(long index)
        {
            return new string(base.GetKeyByIndex(index).ToArray());
        }
    }
}