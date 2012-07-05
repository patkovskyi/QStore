using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Sequences
{
    internal class SequenceSetTransitionSymbolComparer : IComparer<SequenceSetTransition>
    {
        private static SequenceSetTransitionSymbolComparer _default = new SequenceSetTransitionSymbolComparer();
        public static SequenceSetTransitionSymbolComparer Default { get { return _default; } }

        public int Compare(SequenceSetTransition x, SequenceSetTransition y)
        {
            return x.AlphabetIndex.CompareTo(y.AlphabetIndex);
        }
    }
}
