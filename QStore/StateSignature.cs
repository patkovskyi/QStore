namespace QStore
{
    internal class StateSignature
    {
        #region Fields

        internal QSetTransition[] Transitions;

        #endregion

        #region Constructors and Destructors

        public StateSignature(QSetTransition[] transitions)
        {
            this.Transitions = transitions;
        }

        #endregion
    }
}