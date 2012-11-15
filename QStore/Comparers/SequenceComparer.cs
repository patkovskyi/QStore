namespace QStore.Comparers
{
    using System.Collections.Generic;

    internal class SequenceComparer<T> : Comparer<IEnumerable<T>>
    {
        #region Fields

        private readonly IComparer<T> elementComparer;

        #endregion

        #region Constructors and Destructors

        public SequenceComparer(IComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        #endregion

        #region Public Methods and Operators

        public override int Compare(IEnumerable<T> x, IEnumerable<T> y)
        {
            // ReSharper disable PossibleMultipleEnumeration
            if (object.ReferenceEquals(x, y)) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            using (var xCurrent = x.GetEnumerator())
            using (var yCurrent = y.GetEnumerator())
            {
                bool xHasElements = xCurrent.MoveNext(), yHasElements = yCurrent.MoveNext();
                while (xHasElements && yHasElements)
                {
                    int comp = this.elementComparer.Compare(xCurrent.Current, yCurrent.Current);
                    if (comp != 0) return comp;

                    xHasElements = xCurrent.MoveNext();
                    yHasElements = yCurrent.MoveNext();
                }

                return xHasElements.CompareTo(yHasElements);
            }

            // ReSharper restore PossibleMultipleEnumeration
        }

        #endregion
    }
}