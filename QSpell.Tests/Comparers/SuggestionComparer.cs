using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Spellchecker;

namespace QSpell.Tests.Comparers
{
    public class SuggestionComparer<T> : Comparer<Suggestion<T>>
        where T : IComparable<T>
    {
        public override int Compare(Suggestion<T> x, Suggestion<T> y)
        {
            int comp = StringComparer.Ordinal.Compare(x.Correction, y.Correction);
            if (comp == 0)
            {
                comp = x.Cost.CompareTo(y.Cost);
                if (comp == 0)
                {
                    comp = x.Frequency.CompareTo(y.Frequency);
                }
            }
            return comp;
        }
    }
}
