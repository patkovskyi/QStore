namespace QStore
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class SimpleSet
    {        
        public static SimpleSet Create(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Get()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetByPrefix(IEnumerable<char> keyPrefix)
        {
            throw new NotImplementedException();
        }
    }
}