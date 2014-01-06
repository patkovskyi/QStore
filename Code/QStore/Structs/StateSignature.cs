namespace QStore.Structs
{
    internal class StateSignature
    {
        internal bool IsFinal;

        internal QTransition[] Transitions;

        public StateSignature(QTransition[] transitions, bool isFinal)
        {
            this.Transitions = transitions;
            this.IsFinal = isFinal;
        }
    }
}