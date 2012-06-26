using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace QSpell.Sequences
{
    internal class StateSignatureEqualityComparer : IEqualityComparer<StateSignature>
    {
        //[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);

        public bool Equals(StateSignature x, StateSignature y)
        {
            //return x.Transitions.Length != y.Transitions.Length && memcmp(x.Transitions, y.Transitions, x.Transitions.Length) == 0;
            return StructuralComparisons.StructuralEqualityComparer.Equals(x.Transitions, y.Transitions);
        }

        public int GetHashCode(StateSignature obj)
        {
            unchecked
            {
                int hash = 17;
                int len = obj.Transitions.Length;
                hash = hash * 23 + len;
                for (int i = 0; i < obj.Transitions.Length; i++)
                {
                    hash = hash * 23 + (obj.Transitions[i].IsFinal ? 1 : 0);
                    hash = hash * 23 + obj.Transitions[i].AlphabetIndex;
                    hash = hash * 23 + obj.Transitions[i].StateIndex;
                }
                //if (len > 0)
                //{
                //    hash = hash * 23 + (obj.Transitions[0].IsFinal ? 1 : 0);
                //    hash = hash * 23 + obj.Transitions[0].AlphabetIndex;
                //    hash = hash * 23 + obj.Transitions[0].StateIndex;
                //}
                //if (len > 1)
                //{
                //    hash = hash * 23 + (obj.Transitions[len - 1].IsFinal ? 1 : 0);
                //    hash = hash * 23 + obj.Transitions[len - 1].AlphabetIndex;
                //    hash = hash * 23 + obj.Transitions[len - 1].StateIndex;
                //}
                //if (len > 2)
                //{
                //    int mid = obj.Transitions.Length / 2;
                //    hash = hash * 23 + (obj.Transitions[mid].IsFinal ? 1 : 0);
                //    hash = hash * 23 + obj.Transitions[mid].AlphabetIndex;
                //    hash = hash * 23 + obj.Transitions[mid].StateIndex;
                //}
                return hash;
            }
        }
    }
}
