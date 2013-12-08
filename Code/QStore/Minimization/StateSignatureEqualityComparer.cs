namespace QStore.Minimization
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class StateSignatureEqualityComparer : IEqualityComparer<StateSignature>
    {
        private static readonly int TransitionSize = Marshal.SizeOf(typeof(QTransition));

        public bool Equals(StateSignature x, StateSignature y)
        {
            return x.Transitions.Length == y.Transitions.Length
                   && memcmp(x.Transitions, y.Transitions, x.Transitions.Length * TransitionSize) == 0;
        }

        public int GetHashCode(StateSignature obj)
        {
            unchecked
            {
                int hash = 17;
                int len = obj.Transitions.Length;
                hash = (hash * 23) + len;
                for (int i = 0; i < obj.Transitions.Length; i++)
                {
                    hash = (hash * 23) + (obj.Transitions[i].IsFinal ? 1 : 0);
                    hash = (hash * 23) + obj.Transitions[i].Symbol;
                    hash = (hash * 23) + obj.Transitions[i].NextState;
                }

                return hash;
            }
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(QTransition[] b1, QTransition[] b2, long count);
    }
}