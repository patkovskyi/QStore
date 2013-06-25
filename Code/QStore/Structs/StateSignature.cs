namespace QStore.Structs
{
    internal class StateSignature
    {
        internal QSetTransition[] Transitions;

        public StateSignature(QSetTransition[] transitions)
        {
            this.Transitions = transitions;
        }
    }
}