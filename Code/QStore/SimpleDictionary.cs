namespace QStore
{
    using System;
    using System.Collections.Generic;

    public class SimpleDictionary<T>
    {
        public T this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public static SimpleDictionary<T> Create(IEnumerable<KeyValuePair<string, T>> keyValues)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, T>> Get()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, T>> GetByPrefix(IEnumerable<char> prefix)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysByPrefix(IEnumerable<char> keyPrefix)
        {
            throw new NotImplementedException();
        }

        public T TryGetValue(string key, out T value)
        {
            throw new NotImplementedException();
        }
    }
}