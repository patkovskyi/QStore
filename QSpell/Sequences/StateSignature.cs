using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Sequences
{
    internal class StateSignature
    {
        internal SequenceSetTransition[] Transitions;

        public StateSignature(SequenceSetTransition[] transitions)
        {
            Transitions = transitions;
        }
    }
}
