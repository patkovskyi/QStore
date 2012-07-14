using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Extensions;

namespace QSpell.Transducer
{
    internal class Transducer<I, O> : IEnumerable<Tuple<IEnumerable<I>, float, IEnumerable<O>>>
    {
        #region CONSTRUCTORS
        internal static T Create<T>(IEnumerable<Tuple<IEnumerable<Tuple<I, O>>, float>> rules, IComparer<I> inputComparer, IComparer<O> outputComparer)
            where T : Transducer<I, O>, new()
        {
            I[] inputAlphabet = rules.SelectMany(s => s.Item1.Select(p => p.Item1)).Distinct().OrderBy(a => a, inputComparer).ToArray();
            O[] outputAlphabet = rules.SelectMany(s => s.Item1.Select(p => p.Item2)).Distinct().OrderBy(a => a, outputComparer).ToArray();

            var transitions = new List<List<TransducerTransition>>(inputAlphabet.Length) { new List<TransducerTransition>() };
            foreach (var rule in rules)
            {
                Int32 curState = -1, nextState = 0, transitionIndex = -1;
                foreach (var pair in rule.Item1)
                {
                    curState = nextState;
                    //transitionIndex = GetTransitionIndex(transitions[curState], pair.Item1, inputAlphabet, inputComparer, pair.Item2, outputAlphabet, outputComparer, 0, transitions[curState].Count);
                    transitionIndex = GetTransitionIndex(transitions[curState], pair.Item1, inputAlphabet, inputComparer, 0, transitions[curState].Count);
                    if (transitionIndex < 0)
                    {
                        transitionIndex = ~transitionIndex;

                        Int32 inputIndex = inputAlphabet.OptimalSearch(pair.Item1);
                        Int32 outputIndex = outputAlphabet.OptimalSearch(pair.Item2);

                        // add new state
                        nextState = transitions.Count;
                        transitions.Add(new List<TransducerTransition>());

                        // add transition from current state to new state                                                
                        var newTransition = new TransducerTransition(inputIndex, 0, outputIndex, nextState, false);
                        transitions[curState].Insert(transitionIndex, newTransition);
                    }
                    else
                    {
                        nextState = transitions[curState][transitionIndex].StateIndex;
                    }
                }
                var tmp = transitions[curState][transitionIndex];
                transitions[curState][transitionIndex] = new TransducerTransition(tmp.InputAlphabetIndex, rule.Item2, tmp.OutputAlphabetIndex, tmp.StateIndex, true);
            }

            var result = new T();
            result.inputAlphabet = inputAlphabet;
            result.inputComparer = inputComparer;
            result.outputAlphabet = outputAlphabet;
            result.outputComparer = outputComparer;
            result.start = 0;
            result.lowerTransitionIndexes = new int[transitions.Count];
            result.transitions = new TransducerTransition[transitions.Sum(s => s.Count)];

            for (int i = 0, trIndex = 0; i < transitions.Count; i++)
            {
                result.lowerTransitionIndexes[i] = trIndex;
                for (int j = 0; j < transitions[i].Count; j++, trIndex++)
                {
                    result.transitions[trIndex] = transitions[i][j];
                }
            }

            return result;
        }

        public static Transducer<I, O> Create(IEnumerable<Tuple<IEnumerable<Tuple<I, O>>, float>> rules, IComparer<I> inputComparer, IComparer<O> outputComparer)
        {
            return Create<Transducer<I, O>>(rules, inputComparer, outputComparer);
        }
        #endregion

        #region FIELDS
        protected I[] inputAlphabet;
        protected IComparer<I> inputComparer;
        protected O[] outputAlphabet;
        protected IComparer<O> outputComparer;
        protected Int32[] lowerTransitionIndexes;
        protected TransducerTransition[] transitions;
        protected Int32 start;
        #endregion

        #region METHODS
        protected bool TryGetTransition(Int32 fromState, I input, Int32 costIndex, out TransducerTransition transition)
        {
            Int32 lower = lowerTransitionIndexes[fromState];
            Int32 upper = transitions.GetUpperIndex(lowerTransitionIndexes, fromState);            

            // TODO: throw exception if upper - lower > costIndex ?

            Int32 transitionIndex = GetTransitionIndex(transitions, input, inputAlphabet, inputComparer, lower, upper);
            if (transitionIndex >= 0)
            {
                while (transitionIndex > 0 && inputComparer.Compare(inputAlphabet[transitions[transitionIndex].InputAlphabetIndex], input) == 0)
                {
                    transitionIndex--;
                }
                transition = transitions[transitionIndex + costIndex];
                return true;
            }
            else
            {
                transition = default(TransducerTransition);
                return false;
            }
        }

        private static Int32 GetTransitionIndex(IList<TransducerTransition> transitions, I input, I[] inputAlphabet, IComparer<I> inputComparer, double cost, Int32 lower, Int32 upper)
        {
            const Int32 BINARY_SEARCH_THRESHOLD = 5;
            if (upper - lower >= BINARY_SEARCH_THRESHOLD)
            {
                // Binary search
                upper--;
                while (lower <= upper)
                {
                    Int32 middle = (lower + upper) / 2;
                    Int32 comparisonResult = inputComparer.Compare(inputAlphabet[transitions[middle].InputAlphabetIndex], input);

                    if (comparisonResult == 0)
                        return middle;
                    else if (comparisonResult > 0)
                        upper = middle - 1;
                    else
                        lower = middle + 1;
                }

                return ~lower;
            }
            else
            {
                // Linear search
                for (Int32 i = lower; i < upper; i++)
                {
                    Int32 comparisonResult = inputComparer.Compare(inputAlphabet[transitions[i].InputAlphabetIndex], input);

                    if (comparisonResult == 0)
                    {
                        return i;
                    }
                    else if (comparisonResult > 0)
                    {
                        return ~i;
                    }
                }

                return ~upper;
            }
        }

        /// <returns>Transition index or bitwise complement to the index where it should be inserted.</returns>
        //private static Int32 GetTransitionIndex(IList<TransducerTransition> transitions, I input, I[] inputAlphabet, IComparer<I> inputComparer, O output, O[] outputAlphabet, IComparer<O> outputComparer, Int32 lower, Int32 upper)
        //{
        //    const Int32 BINARY_SEARCH_THRESHOLD = 5;
        //    if (upper - lower >= BINARY_SEARCH_THRESHOLD)
        //    {
        //        // Binary search
        //        upper--;
        //        while (lower <= upper)
        //        {
        //            Int32 middle = (lower + upper) / 2;
        //            Int32 comparisonResult = inputComparer.Compare(inputAlphabet[transitions[middle].InputAlphabetIndex], input);
        //            if (comparisonResult == 0)
        //                comparisonResult = outputComparer.Compare(outputAlphabet[transitions[middle].OutputAlphabetIndex], output);

        //            if (comparisonResult == 0)
        //                return middle;
        //            else if (comparisonResult > 0)
        //                upper = middle - 1;
        //            else
        //                lower = middle + 1;
        //        }

        //        return ~lower;
        //    }
        //    else
        //    {
        //        // Linear search
        //        for (Int32 i = lower; i < upper; i++)
        //        {
        //            Int32 comparisonResult = inputComparer.Compare(inputAlphabet[transitions[i].InputAlphabetIndex], input);
        //            if (comparisonResult == 0)
        //                comparisonResult = outputComparer.Compare(outputAlphabet[transitions[i].OutputAlphabetIndex], output);

        //            if (comparisonResult == 0)
        //            {
        //                return i;
        //            }
        //            else if (comparisonResult > 0)
        //            {
        //                return ~i;
        //            }
        //        }

        //        return ~upper;
        //    }
        //}
        #endregion

        #region IEnumerable<Tuple<IEnumerable<I>, float, IEnumerable<O>>>
        public IEnumerator<Tuple<IEnumerable<I>, float, IEnumerable<O>>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
