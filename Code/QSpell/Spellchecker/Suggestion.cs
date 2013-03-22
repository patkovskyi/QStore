
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace QSpell.Spellchecker
{
    [DebuggerDisplay("{debuggerDisplay}")]
    public class Suggestion<T>
        where T : IComparable<T>
    {
        public string Correction { get; protected set; }
        public double Cost { get; protected set; }
        public T Frequency { get; protected set; }

        protected string debuggerDisplay
        {
            get
            {
                return String.Format("Correction: \"{0}\" Cost: {1} Frequency: {2}", Correction, Cost, Frequency);
            }
        }

        public Suggestion(string correction, double cost, T frequency)
        {
            Correction = correction;
            Cost = cost;
            Frequency = frequency;
        }
    }
}
