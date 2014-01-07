namespace QStore.Structs
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public struct QTransition
    {
        [DataMember(Order = 2)]
        internal readonly int StateIndex;

        [DataMember(Order = 1)]
        internal readonly char Symbol;

        public QTransition(char symbol, int nextState)
        {
            this.Symbol = symbol;
            this.StateIndex = nextState;
        }
    }
}