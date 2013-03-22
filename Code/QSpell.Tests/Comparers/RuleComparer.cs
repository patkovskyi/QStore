using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Comparers;

namespace QSpell.Tests.Comparers
{
    public class RuleComparer<I, O> : Comparer<Tuple<IEnumerable<Tuple<I, O>>, float>>
    {
        protected IComparer<I> inputComparer;
        protected IComparer<O> outputComparer;
        protected IComparer<Tuple<I, O>> tupleComparer;
        protected IComparer<IEnumerable<Tuple<I, O>>> tupleSequenceComparer;

        public override int Compare(Tuple<IEnumerable<Tuple<I, O>>, float> x, Tuple<IEnumerable<Tuple<I, O>>, float> y)
        {
            int comp = tupleSequenceComparer.Compare(x.Item1, y.Item1);
            if (comp == 0)
            {
                comp = x.Item2.CompareTo(y.Item2);
            }
            return comp;
        }

        public RuleComparer(IComparer<I> inputComparer, IComparer<O> outputComparer)
        {
            this.inputComparer = inputComparer;
            this.outputComparer = outputComparer;
            tupleComparer = new TupleComparer<I, O>(inputComparer, outputComparer);
            tupleSequenceComparer = new SequenceComparer<Tuple<I, O>>(tupleComparer);
        }
    }
}
