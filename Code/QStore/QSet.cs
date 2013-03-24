namespace QStore
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Extensions;
    using QStore.Structs;

    public class QSet<T> : ISequenceSet<T>, IEnumerable<IEnumerable<T>>
    {
        private T[] alphabet;

        private int[] lowerTransitionIndexes;

        private int start;

        // TODO: think of serialization.        
        private IComparer<T> symbolComparer;

        private SequenceSetTransition[] transitions;

        public static QSet<T> Create(
            IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer, IEqualityComparer<T> equalityComparer)
        {
            var sequencesArray = sequences as T[][] ?? sequences.Select(s => s.ToArray()).ToArray();
            var alphabet = ExtractAlphabet(sequencesArray, comparer, equalityComparer);
            var alphabetDict = alphabet.Select((a, i) => new KeyValuePair<T, int>(a, i))
                                       .ToDictionary(p => p.Key, p => p.Value, equalityComparer);

            var transitions = new List<List<SequenceSetTransition>>(alphabet.Length)
            {
                new List<SequenceSetTransition>()
            };

            foreach (var sequence in sequencesArray)
            {
                int curState = -1, nextState = 0, transitionIndex = -1;
                foreach (var element in sequence)
                {
                    curState = nextState;
                    transitionIndex = GetTransitionIndex(
                        transitions[curState], element, alphabet, comparer, 0, transitions[curState].Count);
                    if (transitionIndex < 0)
                    {
                        transitionIndex = ~transitionIndex;

                        // Int32 alphabetIndex = alphabet.OptimalSearch(element);
                        int alphabetIndex = alphabetDict[element];

                        // add new state
                        nextState = transitions.Count;
                        transitions.Add(new List<SequenceSetTransition>());

                        // add transition from current state to new state                                                
                        var newTransition = new SequenceSetTransition(alphabetIndex, nextState, false);
                        transitions[curState].Insert(transitionIndex, newTransition);
                    }
                    else
                    {
                        nextState = transitions[curState][transitionIndex].StateIndex;
                    }
                }

                var tmp = transitions[curState][transitionIndex];
                transitions[curState][transitionIndex] = new SequenceSetTransition(
                    tmp.AlphabetIndex, tmp.StateIndex, true);
            }

            var result = new QSet<T>
            {
                symbolComparer = comparer, 
                alphabet = alphabet, 
                start = 0, 
                lowerTransitionIndexes = new int[transitions.Count], 
                transitions = new SequenceSetTransition[transitions.Sum(s => s.Count)]
            };

            for (int i = 0, transitionIndex = 0; i < transitions.Count; i++)
            {
                result.lowerTransitionIndexes[i] = transitionIndex;
                for (int j = 0; j < transitions[i].Count; j++, transitionIndex++)
                {
                    result.transitions[transitionIndex] = transitions[i][j];
                }
            }

            return result;
        }

        public static T[] ExtractAlphabet(
            IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer, IEqualityComparer<T> equalityComparer)
        {
            return sequences.SelectMany(s => s).Distinct(equalityComparer).OrderBy(a => a, comparer).ToArray();
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
            return this.Enumerate(this.start).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // TODO: check if "int" is enough
        public int GetIndex(IEnumerable<T> sequence)
        {
            throw new System.NotImplementedException();
        }

        internal bool TrySend(int fromState, T input, out SequenceSetTransition transition)
        {
            int transitionIndex = this.GetTransitionIndex(fromState, input);
            if (transitionIndex >= 0)
            {
                transition = this.transitions[transitionIndex];
                return true;
            }

            transition = default(SequenceSetTransition);
            return false;
        }

        internal bool TrySend(int fromState, IEnumerable<T> inputSequence, out SequenceSetTransition transition)
        {
            int currentState = fromState;
            transition = default(SequenceSetTransition);
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

        protected IEnumerable<IEnumerable<T>> Enumerate(int fromState)
        {
            int lower = this.lowerTransitionIndexes[fromState];
            int upper = this.transitions.GetUpperIndex(this.lowerTransitionIndexes, fromState);

            for (int i = lower; i < upper; i++)
            {
                var tr = this.transitions[i];
                T element = this.alphabet[tr.AlphabetIndex];
                if (tr.IsFinal)
                {
                    yield return new[] { element };
                }

                foreach (var w in this.Enumerate(tr.StateIndex).Select(w => w.Prepend(element)))
                {
                    yield return w;
                }
            }
        }

        /// <returns>Transition index or bitwise complement to the index where it should be inserted.</returns>
        protected int GetTransitionIndex(int fromState, T input)
        {
            int lower = this.lowerTransitionIndexes[fromState];
            int upper = this.transitions.GetUpperIndex(this.lowerTransitionIndexes, fromState);
            return GetTransitionIndex(this.transitions, input, this.alphabet, this.symbolComparer, lower, upper);
        }

        private static int GetTransitionIndex(
            IList<SequenceSetTransition> transitions, T input, T[] alphabet, IComparer<T> comparer, int lower, int upper)
        {
            const int BinarySearchThreshold = 5;
            if (upper - lower >= BinarySearchThreshold)
            {
                // Binary search
                upper--;
                while (lower <= upper)
                {
                    int middle = (lower + upper) / 2;
                    int comparisonResult = comparer.Compare(alphabet[transitions[middle].AlphabetIndex], input);
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

            // Linear search
            for (int i = lower; i < upper; i++)
            {
                int comp = comparer.Compare(alphabet[transitions[i].AlphabetIndex], input);
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