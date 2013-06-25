namespace QStore
{
    using System.Collections.Generic;

    public class QMap<TKey, TValue> : QIndexedSet<TKey>, 
                                      ISequenceMap<TKey, TValue>, 
                                      IEnumerable<KeyValuePair<IEnumerable<TKey>, TValue>>
    {
        public new IEnumerable<KeyValuePair<IEnumerable<TKey>, TValue>> GetByPrefix(IEnumerable<TKey> prefix)
        {
            throw new System.NotImplementedException();
        }

        public new IEnumerator<KeyValuePair<IEnumerable<TKey>, TValue>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TKey> GetKeyByIndex(int index)
        {
            throw new System.NotImplementedException();
        }

        public TValue GetValueByIndex(int index)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(IEnumerable<TKey> key, out TValue value)
        {
            throw new System.NotImplementedException();
        }
    }
}