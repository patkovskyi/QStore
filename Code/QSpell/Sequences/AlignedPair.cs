using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace QSpell.Sequences
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AlignedPair<TKey, TValue>
    {
        public AlignedPair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        private TKey key;
        public TKey Key { get { return key; } }

        private TValue value;
        public TValue Value { get { return value; } }

        public override string ToString()
        {
            return String.Format("<{0},{1}>", Key.ToString(), Value.ToString());
        }
    }
}
