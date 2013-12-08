namespace QStore.Minimization
{
    internal class StateSignature
    {
        internal QTransition[] Transitions;

        public StateSignature(QTransition[] transitions)
        {
            this.Transitions = transitions;
        }
    }
}