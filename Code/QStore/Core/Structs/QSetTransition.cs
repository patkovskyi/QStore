namespace QStore.Core.Structs
{
    public struct QSetTransition
    {
        internal readonly int StateIndex;

        /// <summary>
        /// Highest (32-nd) bit stands for IsFinal.
        /// </summary>
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