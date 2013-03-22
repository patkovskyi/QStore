using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Sequences
{
    interface ISequenceSet<in T>
    {
        bool TrySend(Int32 fromState, T input, out SequenceSetTransition transition);
    }
}
