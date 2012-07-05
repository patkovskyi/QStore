using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Transducer
{
    internal struct TransducerTransition
    {
        /// <summary>
        /// Highest (32-nd) bit stands for IsFinal.
        /// </summary>
        private Int32 _outputAlphabetIndex;        

        internal Int32 InputAlphabetIndex;
        internal float Cost;        
        internal Int32 OutputAlphabetIndex
        {
            get
            {
                return _outputAlphabetIndex & 2147483647;
            }
            set
            {
                _outputAlphabetIndex = (_outputAlphabetIndex & -2147483648) | (value & 2147483647);
            }
        }        
        internal Int32 StateIndex;

        public TransducerTransition(Int32 inputAlphabetIndex, float cost, Int32 outputAlphabetIndex, Int32 stateIndex, bool isFinal)
        {
            InputAlphabetIndex = inputAlphabetIndex;
            Cost = cost;
            _outputAlphabetIndex = (outputAlphabetIndex & 2147483647) | (isFinal ? -2147483648 : 0);
            StateIndex = stateIndex;
        }
    }
}
