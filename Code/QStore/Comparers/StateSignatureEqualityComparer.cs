namespace QStore.Comparers
{
    using System.Collections;
    using System.Collections.Generic;

    using QStore.Structs;

    internal class StateSignatureEqualityComparer : IEqualityComparer<StateSignature>
    {
        public bool Equals(StateSignature x, StateSignature y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x.Transitions, y.Transitions);
        }

        public int GetHashCode(StateSignature obj)
        {
            unchecked
            {
                int hash = 17;
                int len = obj.Transitions.Length;
                hash = (hash * 23) + len;
                hash = (hash * 23) + (obj.IsFinal ? 1 : 0);
                for (int i = 0; i < obj.Transitions.Length; i++)
                {
                    hash = (hash * 23) + obj.Transitions[i].Symbol;
                    hash = (hash * 23) + obj.Transitions[i].StateIndex;
                }

                return hash;
            }
        }
    }
}