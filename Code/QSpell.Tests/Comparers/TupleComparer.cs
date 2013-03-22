using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Tests.Comparers
{
    public class TupleComparer<T1, T2> : Comparer<Tuple<T1, T2>>
    {
        protected IComparer<T1> firstComparer;
        protected IComparer<T2> secondComparer;

        public override int Compare(Tuple<T1, T2> x, Tuple<T1, T2> y)
        {
            int comp = firstComparer.Compare(x.Item1, y.Item1);
            if (comp == 0)
            {
                comp = secondComparer.Compare(x.Item2, y.Item2);
            }
            return comp;
        }

        public TupleComparer(IComparer<T1> firstComparer, IComparer<T2> secondComparer)
        {
            this.firstComparer = firstComparer;
            this.secondComparer = secondComparer;
        }
    }
}
