
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Spellchecker
{
    public class Suggestion<T>
        where T : IComparable<T>
    {
        public String Correction { get; protected set; }
        public double Cost { get; protected set; }
        public T Frequency { get; protected set; } 

        public Suggestion(String correction, double cost, T frequency)
        {
            Correction = correction;
            Cost = cost;
            Frequency = frequency;
        }
    }
}
