using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace QSpell.Spellchecker
{
    [DebuggerDisplay("{debuggerDisplay}")]
    public class Rule<I, O>
    {
        /// <summary>
        /// Wrong (input) sequence.
        /// </summary>
        public I[] Left { get; protected set; }

        /// <summary>
        /// Typo cost.
        /// </summary>
        public double Cost { get; protected set; }

        /// <summary>
        /// Corrected (output) sequence.
        /// </summary>
        public O[] Right { get; protected set; }

        private string debuggerDisplay
        {
            get
            {
                return String.Format("Left - {{0}} Cost - {1} Right - {{2}}", String.Join(",", Left), Cost, String.Join(",", Right));
            }
        }

        public Rule(I[] left, double cost, O[] right)
        {
            Left = left;
            Cost = cost;
            Right = right;
        }
    }
}
