using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Sequences
{
    internal class SequenceSetTransitionComparer : Comparer<SequenceSetTransition>
    {
        private static SequenceSetTransitionComparer _default = new SequenceSetTransitionComparer();
        public static new SequenceSetTransitionComparer Default { get { return _default; } }

        public override int Compare(SequenceSetTransition x, SequenceSetTransition y)
        {
            Int32 comp = x.IsFinal.CompareTo(y.IsFinal);
            if (comp == 0)
            {
                comp = x.AlphabetIndex.CompareTo(y.AlphabetIndex);
                if (comp == 0)
                {
                    comp = x.StateIndex.CompareTo(y.StateIndex);
                }
            }
            return comp;
        }
    }
}
