using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Tests
{
    public class SequenceComparer<T> : Comparer<IEnumerable<T>>
    {
        private IComparer<T> elementComparer;

        public override int Compare(IEnumerable<T> x, IEnumerable<T> y)
        {
            var xCur = x.GetEnumerator();
            var yCur = y.GetEnumerator();
            int comp = 0;
            bool xHasElements = xCur.MoveNext(), yHasElements = yCur.MoveNext();
            while (xHasElements && yHasElements)
            {
                comp = elementComparer.Compare(xCur.Current, yCur.Current);
                if (comp != 0)
                {
                    return comp;
                }
                xHasElements = xCur.MoveNext();
                yHasElements = yCur.MoveNext();
            }

            return xHasElements.CompareTo(yHasElements);
        }

        public SequenceComparer(IComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer;
        }
    }
}
