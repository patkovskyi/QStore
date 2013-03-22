namespace QStore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Structs;

    public class QSet<T> : ISequenceSet<T>, IEnumerable<IEnumerable<T>>
    {
        private T[] alphabet;

        private int[] lowerTransitionIndexes;

        private int start;

        // TODO: think of serialization.        
        private IComparer<T> symbolComparer;

        private SequenceSetTransition[] transitions;

        public static QSet<T> Create(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer)
        {
            var sequencesArray = sequences as T[][] ?? sequences.Select(s => s.ToArray()).ToArray();
            return Create(sequencesArray, comparer, ExtractAlphabet(sequencesArray, comparer));
        }

        // TODO: should throw some meaningful exception when alphabet is not OK
        public static QSet<T> Create(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer, T[] alphabet)
        {
            var alphabetDict = alphabet.Select((c, i) => new Tuple<T, int>(c, i))
                                       .ToDictionary(t => t.Item1, t => t.Item2);

            var transitions = new List<List<SequenceSetTransition>>(alphabet.Length)
            {
                new List<SequenceSetTransition>()
            };

            foreach (var sequence in sequences)
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

        public static T[] ExtractAlphabet(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer)
        {
            return sequences.SelectMany(s => s).Distinct().OrderBy(a => a, comparer).ToArray();
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
            throw new System.NotImplementedException();
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

        protected void SomeProtest()
        {
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
            else
            {
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
}