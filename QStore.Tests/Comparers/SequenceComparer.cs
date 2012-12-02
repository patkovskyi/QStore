namespace QStore.Tests.Comparers
{
    using System.Collections.Generic;

    public class SequenceComparer<T> : NullComparer<IEnumerable<T>>
    {
        private readonly IComparer<T> elementComparer;

        public SequenceComparer(IComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        public override int Compare(IEnumerable<T> x, IEnumerable<T> y)
        {
            if (x == null || y == null)
            {
                return base.Compare(x, y);
            }

            var xCur = x.GetEnumerator();
            var yCur = y.GetEnumerator();
            bool xHasElements = xCur.MoveNext();
            bool yHasElements = yCur.MoveNext();
            while (xHasElements && yHasElements)
            {
                int comp = this.elementComparer.Compare(xCur.Current, yCur.Current);
                if (comp != 0)
                {
                    return comp;
                }

                xHasElements = xCur.MoveNext();
                yHasElements = yCur.MoveNext();
            }

            return xHasElements.CompareTo(yHasElements);
        }
    }
}