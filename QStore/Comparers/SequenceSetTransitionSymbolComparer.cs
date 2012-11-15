namespace QSpell.Comparers
{
    using System.Collections.Generic;

    using QStore;

    internal class QSetTransitionSymbolComparer : Comparer<QSetTransition>
    {
        #region Static Fields

        private static readonly QSetTransitionSymbolComparer DefaultInstance = new QSetTransitionSymbolComparer();

        #endregion

        #region Public Properties

        public static new IComparer<QSetTransition> Default
        {
            get
            {
                return DefaultInstance;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override int Compare(QSetTransition x, QSetTransition y)
        {
            return x.AlphabetIndex.CompareTo(y.AlphabetIndex);
        }

        #endregion
    }
}