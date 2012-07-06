using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Spellchecker
{
    internal class QStackObject
    {
        internal Int32 State { get; private set; }
        internal bool IsFinal { get; private set; }
        internal double Cost { get; private set; }
        internal Int32 CharIndex { get; private set; }
        internal QStackObject Prev { get; private set; }
        internal Rule PrevUsedRule { get; private set; }
        internal Rule[] CachedRules { get; private set; }
        internal Int32 LastRuleIndex { get; set; }

        public QStackObject(Int32 state, bool isFinal, double cost, Int32 charIndex, QStackObject prev, Rule prevUsedRule, Rule[] cachedRules, Int32 lastRuleIndex)
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

        public string Backtrace(string input)
        {
            var sb = new StringBuilder();
            for (QStackObject current = this; this.Prev != null; current = current.Prev)
            {
                sb.Insert(0, PrevUsedRule.Right);
            }
            return sb.ToString();
        }
    }
}
