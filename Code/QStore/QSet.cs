namespace QStore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Extensions;
    using QStore.Structs;

    public class QSet<T> : ISequenceSet<T>, IEnumerable<IEnumerable<T>>
    {
        private T[] alphabet;
        
        private IComparer<T> comparer;

        private int rootState;

        private int[] stateStarts;

        private QSetTransition[] transitions;        

        public static QSet<T> Create(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer)
        {
            // avoid multiple enumeration
            var seqArray = sequences as T[][] ?? sequences.Select(s => s.ToArray()).ToArray();

            T[] alphabet;
            SortedDictionary<T, int> alphabetDict;
            ExtractAlphabet(seqArray, comparer, out alphabet, out alphabetDict);

            // outer list represents states, inner lists represent transitions from this state
            // State1 -------> State2 -------> ...... -------> StateN
            // |               |
            // a->State2       b->State3
            // |               |
            // d->State5       e->State4,final
            // |
            // z->StateN,final
            // capacity is set to alphabet.Length just to avoid few initial resizings
            var transitions = new List<List<QSetTransition>>(alphabet.Length) { new List<QSetTransition>() };

            foreach (var sequence in seqArray)
            {
                int nextState = 0, transitionIndex = -1;
                List<QSetTransition> currentStateTransitions = null;
                foreach (var symbol in sequence)
                {
                    currentStateTransitions = transitions[nextState];
                    transitionIndex = GetTransitionIndex(currentStateTransitions, symbol, alphabet, comparer);
                    if (transitionIndex >= 0)
                    {
                        // transition from currentState by symbol already exists
                        nextState = currentStateTransitions[transitionIndex].StateIndex;
                    }
                    else
                    {
                        // transition from currentState by symbol doesn't exist
                        transitionIndex = ~transitionIndex;

                        // find alphabetIndex for symbol
                        int alphabetIndex = alphabetDict[symbol];

                        // add new state
                        nextState = transitions.Count;
                        transitions.Add(new List<QSetTransition>());

                        // add transition from current state to new state                                                
                        var newTransition = new QSetTransition(alphabetIndex, nextState, false);

                        // I know this Insert in the middle of the List<> doesn't look very clever,
                        // but I tried SortedDictionary and LinkedList approach: they are slower in practice
                        currentStateTransitions.Insert(transitionIndex, newTransition);
                    }
                }
                
                if (currentStateTransitions == null)
                {
                    throw new ArgumentException("Empty sequences are not allowed!");
                }

                if (currentStateTransitions[transitionIndex].IsFinal)
                {
                    throw new ArgumentException(
                        string.Format(
                            "An element with Key = \"{0}\" already exists.",
                            string.Concat(sequence.Select(e => e.ToString()))));
                }
                
                // mark last transition in this sequence as final
                currentStateTransitions[transitionIndex] = currentStateTransitions[transitionIndex].MakeFinal();
            }        

            var result = new QSet<T>
            {
                comparer = comparer, 
                alphabet = alphabet, 
                rootState = 0, 
                stateStarts = new int[transitions.Count], 
                transitions = new QSetTransition[transitions.Sum(s => s.Count)]
            };

            for (int i = 0, transitionIndex = 0; i < transitions.Count; i++)
            {
                result.stateStarts[i] = transitionIndex;
                for (int j = 0; j < transitions[i].Count; j++, transitionIndex++)
                {
                    result.transitions[transitionIndex] = transitions[i][j];
                }
            }

            return result;
        }        

        public bool Contains(IEnumerable<T> sequence)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetByIndex(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IEnumerable<T>> GetByPrefix(IEnumerable<T> prefix)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return this.Enumerate(this.rootState).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public long GetIndex(IEnumerable<T> sequence)
        {
            throw new System.NotImplementedException();
        }

        internal bool TrySend(int fromState, T input, out QSetTransition transition)
        {
            int transitionIndex = this.GetTransitionIndex(fromState, input);
            if (transitionIndex >= 0)
            {
                transition = this.transitions[transitionIndex];
                return true;
            }

            transition = default(QSetTransition);
            return false;
        }

        internal bool TrySend(int fromState, IEnumerable<T> inputSequence, out QSetTransition transition)
        {
            int currentState = fromState;
            transition = default(QSetTransition);
            foreach (var input in inputSequence)
            {
                if (this.TrySend(currentState, input, out transition))
                {
                    currentState = transition.StateIndex;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }        

        protected IEnumerable<T[]> Enumerate(int fromState, Stack<int> fromStack = null)
        {            
            fromStack = fromStack ?? new Stack<int>();
            var toStack = new Stack<int>();
            int lower = this.stateStarts[fromState];
            int upper = this.transitions.GetUpperIndex(this.stateStarts, fromState);
            fromStack.Push(lower);
            toStack.Push(upper);

            while (fromStack.Count > 0)
            {
                lower = fromStack.Pop();
                upper = toStack.Peek();                

                if (lower < upper)
                {
                    fromStack.Push(lower + 1);

                    if (this.transitions[lower].IsFinal)
                    {
                        var tmp = new int[fromStack.Count];
                        fromStack.CopyTo(tmp, 0);
                        var res = new T[fromStack.Count];
                        for (int i = 0; i < res.Length; i++)
                        {
                            res[i] = this.alphabet[this.transitions[tmp[res.Length - i - 1] - 1].AlphabetIndex];
                        }

                        yield return res;                        
                    } 

                    int nextState = this.transitions[lower].StateIndex;
                    int nextLower = this.stateStarts[nextState];
                    int nextUpper = this.transitions.GetUpperIndex(this.stateStarts, nextState);
                    if (nextLower < nextUpper)
                    {
                        fromStack.Push(nextLower);
                        toStack.Push(nextUpper);
                    }                                                       
                }
                else
                {                    
                    toStack.Pop();
                }
            }
        }        

        /// <returns>Transition index or bitwise complement to the index where it should be inserted.</returns>
        protected int GetTransitionIndex(int fromState, T symbol)
        {
            int lower = this.stateStarts[fromState];
            int upper = this.transitions.GetUpperIndex(this.stateStarts, fromState);
            return GetTransitionIndex(this.transitions, symbol, this.alphabet, this.comparer, lower, upper);
        }

        private static void ExtractAlphabet(
            IEnumerable<IEnumerable<T>> sequences, 
            IComparer<T> comparer, 
            out T[] alphabet, 
            out SortedDictionary<T, int> alphabetDictionary)
        {
            alphabetDictionary = new SortedDictionary<T, int>(comparer);
            foreach (var sequence in sequences)
            {
                foreach (T element in sequence)
                {
                    if (!alphabetDictionary.ContainsKey(element))
                    {
                        alphabetDictionary.Add(element, 0);
                    }
                }
            }

            alphabet = alphabetDictionary.Keys.ToArray();

            for (int i = 0; i < alphabet.Length; i++)
            {
                alphabetDictionary[alphabet[i]] = i;
            }
        }        

        private static int GetTransitionIndex(
            IList<QSetTransition> transitions, T symbol, T[] alphabet, IComparer<T> comparer)
        {
            return GetTransitionIndex(transitions, symbol, alphabet, comparer, 0, transitions.Count);
        }

        private static int GetTransitionIndex(
            IList<QSetTransition> transitions, T symbol, T[] alphabet, IComparer<T> comparer, int lower, int upper)
        {
            const int BinarySearchThreshold = 5;
            if (upper - lower >= BinarySearchThreshold)
            {
                // binary search
                upper--;
                while (lower <= upper)
                {
                    int middle = (lower + upper) / 2;
                    int comparisonResult = comparer.Compare(alphabet[transitions[middle].AlphabetIndex], symbol);
                    if (comparisonResult == 0)
                    {
                        return middle;
                    }

                    if (comparisonResult > 0)
                    {
                        upper = middle - 1;
                    }
                    else
                    {
                        lower = middle + 1;
                    }
                }

                return ~lower;
            }

            // linear search
            for (int i = lower; i < upper; i++)
            {
                int comp = comparer.Compare(alphabet[transitions[i].AlphabetIndex], symbol);
                if (comp == 0)
                {
                    return i;
                }

                if (comp > 0)
                {
                    return ~i;
                }
            }

            return ~upper;
        }
    }
}