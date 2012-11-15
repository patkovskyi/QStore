namespace QStore.Comparers
{
    using System.Collections;
    using System.Collections.Generic;

    internal class StateSignatureEqualityComparer : EqualityComparer<StateSignature>
    {
        #region Public Methods and Operators

        public override bool Equals(StateSignature x, StateSignature y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x.Transitions, y.Transitions);
        }

        public override int GetHashCode(StateSignature obj)
        {
            if (obj == null) return 0;
            unchecked
            {
                int hash = 17;
                int len = obj.Transitions.Length;
                hash = (hash * 23) + len;
                foreach (var t in obj.Transitions)
                {
                    hash = (hash * 23) + (t.Final ? 1 : 0);
                    hash = (hash * 23) + t.AlphabetIndex;
                    hash = (hash * 23) + t.StateIndex;
                }

                return hash;
            }
        }

        #endregion
    }
}