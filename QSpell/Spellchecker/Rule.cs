using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Spellchecker
{
    public class Rule
    {
        /// <summary>
        /// Wrong string sequence.
        /// </summary>
        public String Left { get; protected set; }

        /// <summary>
        /// Typo cost.
        /// </summary>
        public double Cost { get; protected set; }

        /// <summary>
        /// Corrected string sequence.
        /// </summary>
        public String Right { get; protected set; }        

        public Rule(String left, double cost, String right)
        {
            Left = left;
            Cost = cost;
            Right = right;
        }
    }
}
