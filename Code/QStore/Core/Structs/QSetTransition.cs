namespace QStore.Core.Structs
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public struct QSetTransition
    {
        [DataMember(Order = 1)]
        internal readonly int StateIndex;

        /// <summary>
        /// Highest (32-nd) bit stands for IsFinal.
        /// </summary>
        [DataMember(Order = 2)]
        private readonly int alphabetIndex;

        public QSetTransition(int alphabetIndex, int stateIndex, bool isFinal)
        {
            this.alphabetIndex = (alphabetIndex & 2147483647) | (isFinal ? -2147483648 : 0);
            this.StateIndex = stateIndex;
        }

        internal int AlphabetIndex
        {
            get
            {
                return this.alphabetIndex & 2147483647;
            }
        }

        internal bool IsFinal
        {
            get
            {
                return (this.alphabetIndex & -2147483648) != 0;
            }
        }

        internal QSetTransition MakeFinal()
        {
            return new QSetTransition(this.AlphabetIndex, this.StateIndex, true);
        }
    }
}