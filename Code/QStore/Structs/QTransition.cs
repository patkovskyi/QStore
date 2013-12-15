namespace QStore.Structs
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public struct QTransition
    {
        [DataMember(Order = 1)]
        internal readonly char Symbol;

        /// <summary>
        /// Highest (32-nd) bit stands for IsFinal.
        /// </summary>
        [DataMember(Order = 2)]
        private readonly int nextState;

        public QTransition(char symbol, int nextState, bool isFinal)
        {
            this.Symbol = symbol;
            this.nextState = (nextState & 2147483647) | (isFinal ? -2147483648 : 0);
        }

        internal int StateIndex
        {
            get
            {
                return this.nextState & 2147483647;
            }
        }

        internal bool IsFinal
        {
            get
            {
                return (this.nextState & -2147483648) != 0;
            }
        }

        internal QTransition MakeFinal()
        {
            return new QTransition(this.Symbol, this.StateIndex, true);
        }
    }
}