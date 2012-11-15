namespace QStore
{
    /// <summary>
    /// The sequence set transition.
    /// </summary>
    public struct QSetTransition
    {
        #region Fields

        /// <summary>
        /// The state index.
        /// </summary>
        internal int StateIndex;

        /// <summary>
        /// Highest (32-nd) bit stands for Final.
        /// </summary>
        private int alphabetIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QSetTransition"/> struct.
        /// </summary>
        /// <param name="alphabetIndex">
        /// The alphabet index.
        /// </param>
        /// <param name="stateIndex">
        /// The state index.
        /// </param>
        /// <param name="final">
        /// The is final.
        /// </param>
        public QSetTransition(int alphabetIndex, int stateIndex, bool final)
        {
            this.alphabetIndex = (alphabetIndex & 2147483647) | (final ? -2147483648 : 0);
            this.StateIndex = stateIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the alphabet index.
        /// </summary>
        internal int AlphabetIndex
        {
            get
            {
                return this.alphabetIndex & 2147483647;
            }

            set
            {
                this.alphabetIndex = (this.alphabetIndex & -2147483648) | (value & 2147483647);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is final.
        /// </summary>
        internal bool Final
        {
            get
            {
                return (this.alphabetIndex & -2147483648) != 0;
            }

            set
            {
                this.alphabetIndex = (this.alphabetIndex & 2147483647) | (value ? -2147483648 : 0);
            }
        }

        #endregion
    }
}