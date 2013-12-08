namespace QStore
{
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;

    [XmlType]    
    public struct QTransition
    {
        [XmlElement(Order = 1)]        
        internal int NextState;

        [XmlElement(Order = 2)]        
        internal char Symbol;

        [XmlElement(Order = 3)]        
        internal bool IsFinal;              

        public QTransition(char symbol, int nextState, bool isFinal)
        {
            this.Symbol = symbol;
            this.NextState = nextState;
            this.IsFinal = isFinal;
        }

        public QTransition MakeFinal()
        {
            return new QTransition(this.Symbol, this.NextState, true);
        }
    }
}