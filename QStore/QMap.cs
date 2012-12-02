namespace QStore
{
    using System.Collections;
    using System.Collections.Generic;

    public class QMap<TKey, TValue> : ISequenceMap<TKey, TValue>, IEnumerable<KeyValuePair<IEnumerable<TKey>, TValue>>
    {
        public bool Contains(IEnumerable<TKey> sequence)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<KeyValuePair<IEnumerable<TKey>, TValue>> GetByPrefix(IEnumerable<TKey> prefix)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<KeyValuePair<IEnumerable<TKey>, TValue>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int GetIndex(IEnumerable<TKey> sequence)
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