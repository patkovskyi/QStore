namespace QStore.Comparers
{
    using System.Collections.Generic;

    internal class QSetTransitionComparer : Comparer<QSetTransition>
    {
        #region Public Methods and Operators

        public override int Compare(QSetTransition x, QSetTransition y)
        {
            int comp = x.Final.CompareTo(y.Final);
            if (comp == 0)
            {
                comp = x.AlphabetIndex.CompareTo(y.AlphabetIndex);
                if (comp == 0) comp = x.StateIndex.CompareTo(y.StateIndex);
            }

            return comp;
        }

        #endregion
    }
}