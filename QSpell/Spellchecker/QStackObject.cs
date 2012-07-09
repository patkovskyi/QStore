using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Spellchecker
{
    internal class QStackObject<I, O>
    {
        internal Int32 State { get; private set; }
        internal bool IsFinal { get; private set; }
        internal double Cost { get; private set; }
        internal Int32 CharIndex { get; private set; }
        internal QStackObject<I, O> Prev { get; private set; }
        internal Rule<I, O> PrevUsedRule { get; private set; }
        internal Rule<I, O>[] CachedRules { get; set; }
        internal Int32 LastRuleIndex { get; set; }

        public QStackObject(Int32 state, bool isFinal, double cost, Int32 charIndex, QStackObject<I, O> prev, Rule<I, O> prevUsedRule, Rule<I, O>[] cachedRules, Int32 lastRuleIndex)
        {
            State = state;
            IsFinal = isFinal;
            Cost = cost;
            CharIndex = charIndex;
            Prev = prev;
            PrevUsedRule = prevUsedRule;
            CachedRules = cachedRules;
            LastRuleIndex = lastRuleIndex;
        }

        public List<O> Backtrace()
        {
            var result = new List<O>();
            for (QStackObject<I, O> current = this; current.Prev != null; current = current.Prev)
            {
                result.InsertRange(0, current.PrevUsedRule.Right);
            }
            return result;
        }
    }
}
