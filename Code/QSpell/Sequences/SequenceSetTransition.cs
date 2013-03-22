using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace QSpell.Sequences
{
    [ProtoContract]
    internal struct SequenceSetTransition
    {
        [ProtoMember(1)]
        /// <summary>
        /// Highest (32-nd) bit stands for IsFinal.
        /// </summary>
        private Int32 _alphabetIndex;

        internal Int32 AlphabetIndex
        {
            get
            {
                return _alphabetIndex & 2147483647;
            }
            set
            {
                _alphabetIndex = (_alphabetIndex & -2147483648) | (value & 2147483647);
            }
        }
        internal bool IsFinal
        {
            get
            {
                return (_alphabetIndex & -2147483648) != 0;
            }
            set
            {
                _alphabetIndex = (_alphabetIndex & 2147483647) | (value ? -2147483648 : 0);
            }
        }
        [ProtoMember(2)]
        internal Int32 StateIndex;

        public SequenceSetTransition(Int32 alphabetIndex, Int32 stateIndex, bool isFinal)
        {
            _alphabetIndex = (alphabetIndex & 2147483647) | (isFinal ? -2147483648 : 0);
            StateIndex = stateIndex;
        }
    }
}
