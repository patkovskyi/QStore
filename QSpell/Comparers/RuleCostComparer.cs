using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Spellchecker;

namespace QSpell.Comparers
{
    internal class RuleCostComparer<I, O> : Comparer<Rule<I, O>>
    {
        public override int Compare(Rule<I, O> x, Rule<I, O> y)
        {
            return x.Cost.CompareTo(y.Cost);
        }
    }
}
