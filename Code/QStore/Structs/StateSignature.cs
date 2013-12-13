namespace QStore.Structs
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