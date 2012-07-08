
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Spellchecker
{
    public class Suggestion<T>
        where T : IComparable<T>
    {
        public string Correction { get; protected set; }
        public double Cost { get; protected set; }
        public T Frequency { get; protected set; } 

        public Suggestion(string correction, double cost, T frequency)
        {
            Correction = correction;
            Cost = cost;
            Frequency = frequency;
        }
    }
}
