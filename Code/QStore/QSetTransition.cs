namespace QStore
{
    using System.Xml.Serialization;

    [XmlType]
    public struct QSetTransition
    {
        [XmlElement(Order = 3)]
        internal bool IsFinal;

        [XmlElement(Order = 1)]
        internal int NextState;

        [XmlElement(Order = 2)]
        internal char Symbol;
    }
}