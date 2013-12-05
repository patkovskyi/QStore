namespace QStore
{
    using System.Xml.Serialization;

    [XmlType]
    internal struct QTransition
    {
        [XmlElement(Order = 3)]
        internal bool IsFinal;

        [XmlElement(Order = 1)]
        internal int NextState;

        [XmlElement(Order = 2)]
        internal char Symbol;

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