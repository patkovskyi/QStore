namespace QStore
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType]
    public class QSet
    {        
        [XmlElement(Order = 1)]
        internal QTransition RootTransition;

        [XmlElement(Order = 2)]
        internal int[] StateStarts;

        [XmlElement(Order = 3)]
        internal QTransition[] QTransitions;

        public IComparer<char> Comparer
        {
            get
            {
                return Comparer<char>.Default;
            }            
        }

        [XmlElement(Order = 5)]
        private int Count { get; set; }

        public static QSet Create(IEnumerable<string> keys)
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

        private void Minimize()
        {
            throw new NotImplementedException();
        }
    }
}