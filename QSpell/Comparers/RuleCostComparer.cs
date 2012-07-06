using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Spellchecker;

namespace QSpell.Comparers
{
    public class RuleCostComparer : Comparer<Rule>
    {
        public override int Compare(Rule x, Rule y)
        {
            return x.Cost.CompareTo(y.Cost);
        }
    }
}
