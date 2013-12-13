namespace QStore
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;

    using QStore.Comparers;
    using QStore.Extensions;
    using QStore.Minimization;
    using QStore.Structs;

    [DataContract]
    [Serializable]
    public class QStringSet
    {
        [DataMember(Order = 1)]
        protected internal char[] Alphabet;

        [NonSerialized]
        protected internal IComparer<char> ComparerField;

        [DataMember(Order = 2)]
        protected internal QTransition RootTransition;

        [DataMember(Order = 3)]
        protected internal int[] StateStarts;

        [DataMember(Order = 4)]
        protected internal QTransition[] Transitions;

        public IComparer<char> Comparer
        {
            get
            {
                return this.ComparerField;
            }
        }

        [DataMember(Order = 5)]
        public int Count { get; protected internal set; }

        public static QStringSet Create(IEnumerable<IEnumerable<char>> sequences, IComparer<char> comparer)
        {
            return Create<QStringSet>(sequences, comparer);
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            QTransition transition;
            if (this.TrySendSequence(this.RootTransition, sequence, out transition))
            {
                return transition.IsFinal;
            }

            return false;
        }

        public IEnumerable<string> Enumerate()
        {
            return this.Enumerate(this.RootTransition);
        }

        public IEnumerable<string> EnumerateByPrefix(IEnumerable<char> prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            QTransition transition;
            var fromStack = new Stack<int>();
            if (this.TrySendSequence(this.RootTransition, prefix, out transition, fromStack))
            {
                return this.Enumerate(transition, fromStack);
            }

            return Enumerable.Empty<string>();
        }

        public void SetComparer(IComparer<char> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            this.ComparerField = comparer;

            // TODO: safety check?
        }

        protected internal IEnumerable<string> Enumerate(QTransition fromTransition, Stack<int> fromStack = null)
        {
            if (fromTransition.IsFinal)
            {
                yield return
                    fromStack == null
                        ? string.Empty
                        : new string(
                            fromStack.Reverse()
                                .Select(i => this.Alphabet[this.Transitions[i - 1].AlphabetIndex])
                                .ToArray());
            }

            fromStack = fromStack ?? new Stack<int>();
            var toStack = new Stack<int>();
            int lower = this.StateStarts[fromTransition.StateIndex];
            int upper = this.Transitions.GetUpperIndex(this.StateStarts, fromTransition.StateIndex);
            fromStack.Push(lower);
            toStack.Push(upper);

            while (toStack.Count > 0)
            {
                lower = fromStack.Pop();
                upper = toStack.Peek();

                if (lower < upper)
                {
                    fromStack.Push(lower + 1);

                    if (this.Transitions[lower].IsFinal)
                    {
                        var tmp = new int[fromStack.Count];
                        fromStack.CopyTo(tmp, 0);
                        var res = new char[fromStack.Count];
                        for (int i = 0; i < res.Length; i++)
                        {
                            res[i] = this.Alphabet[this.Transitions[tmp[res.Length - i - 1] - 1].AlphabetIndex];
                        }

                        yield return new string(res);
                    }

                    int nextState = this.Transitions[lower].StateIndex;
                    int nextLower = this.StateStarts[nextState];
                    int nextUpper = this.Transitions.GetUpperIndex(this.StateStarts, nextState);
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

        protected internal bool TrySend(int fromState, char input, out int transitionIndex)
        {
            transitionIndex = this.GetTransitionIndex(fromState, input);
            if (transitionIndex >= 0)
            {
                return true;
            }

            return false;
        }

        protected internal bool TrySendSequence(
            QTransition fromTransition,
            IEnumerable<char> sequence,
            out QTransition transition,
            Stack<int> nextTransitions = null)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            transition = fromTransition;

            foreach (var element in sequence)
            {
                int transitionIndex;
                if (this.TrySend(transition.StateIndex, element, out transitionIndex))
                {
                    transition = this.Transitions[transitionIndex];

                    if (nextTransitions != null)
                    {
                        nextTransitions.Push(transitionIndex + 1);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        protected static TSet Create<TSet>(IEnumerable<IEnumerable<char>> sequences, IComparer<char> comparer)
            where TSet : QStringSet, new()
        {
            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            // ReSharper disable PossibleMultipleEnumeration
            char[] alphabet;
            SortedDictionary<char, int> alphabetDict;
            ExtractAlphabet(sequences, comparer, out alphabet, out alphabetDict);

            // outer list represents states, inner lists represent transitions from this state
            // State1 -------> State2 -------> ...... -------> StateN
            // |               |
            // a->State2       b->State3
            // |               |
            // d->State5       e->State4,final
            // |
            // z->StateN,final
            // capacity is set to alphabet.Length just to avoid few initial resizings
            var rootTransition = default(QTransition);
            var transitions = new List<List<QTransition>>(alphabet.Length) { new List<QTransition>() };

            int sequenceCounter = 0;
            foreach (var sequence in sequences)
            {
                int nextState = 0, transitionIndex = -1;
                List<QTransition> currentStateTransitions = null;
                foreach (var symbol in sequence)
                {
                    currentStateTransitions = transitions[nextState];
                    transitionIndex = GetTransitionIndex(
                        currentStateTransitions, symbol, alphabet, comparer, 0, currentStateTransitions.Count);
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
                        transitions.Add(new List<QTransition>());

                        // add transition from current state to new state                                                
                        var newTransition = new QTransition(alphabetIndex, nextState, false);

                        // I know this Insert in the middle of the List<> doesn't look very clever,
                        // but I tried SortedDictionary and LinkedList approach: they are slower in practice
                        currentStateTransitions.Insert(transitionIndex, newTransition);
                    }
                }

                // mark last transition in this sequence as final
                // throw an exception if it's already final
                if (currentStateTransitions == null)
                {
                    if (rootTransition.IsFinal)
                    {
                        throw new ArgumentException(string.Format(Messages.DuplicateKey, string.Empty));
                    }

                    rootTransition = rootTransition.MakeFinal();
                }
                else
                {
                    if (currentStateTransitions[transitionIndex].IsFinal)
                    {
                        throw new ArgumentException(
                            string.Format(Messages.DuplicateKey, string.Concat(sequence.Select(e => e.ToString(CultureInfo.InvariantCulture)))));
                    }

                    currentStateTransitions[transitionIndex] = currentStateTransitions[transitionIndex].MakeFinal();
                }

                ++sequenceCounter;
            }

            var result = new TSet
            {
                ComparerField = comparer,
                Alphabet = alphabet,
                RootTransition = rootTransition,
                StateStarts = new int[transitions.Count],
                Transitions = new QTransition[transitions.Sum(s => s.Count)],
                Count = sequenceCounter
            };

            for (int i = 0, transitionIndex = 0; i < transitions.Count; i++)
            {
                result.StateStarts[i] = transitionIndex;
                for (int j = 0; j < transitions[i].Count; j++, transitionIndex++)
                {
                    result.Transitions[transitionIndex] = transitions[i][j];
                }
            }

            result.Minimize();
            return result;

            // ReSharper restore PossibleMultipleEnumeration
        }

        private static void ExtractAlphabet(
            IEnumerable<IEnumerable<char>> sequences,
            IComparer<char> comparer,
            out char[] alphabet,
            out SortedDictionary<char, int> alphabetDictionary)
        {
            alphabetDictionary = new SortedDictionary<char, int>(comparer);
            foreach (var sequence in sequences)
            {
                foreach (char element in sequence)
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
            IList<QTransition> transitions, char symbol, char[] alphabet, IComparer<char> comparer, int lower, int upper)
        {
            const int BinarySearchThreshold = 1;
            if (upper - lower >= BinarySearchThreshold)
            {
                // binary search
                upper--;
                while (lower <= upper)
                {
                    int middle = lower + ((upper - lower) >> 1);
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
            int i;
            for (i = lower; i < upper; i++)
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

            return ~i;
        }

        private MergeList GetMergeList()
        {
            var merge = new MergeList(this.StateStarts.Length, this.Transitions.Length);
            for (int i = 0; i < this.Transitions.Length; i++)
            {
                merge.Add(i, this.Transitions[i].StateIndex);
            }

            return merge;
        }

        /// <returns>Transition index or bitwise complement to the index where it should be inserted.</returns>
        private int GetTransitionIndex(int fromState, char symbol)
        {
            int lower = this.StateStarts[fromState];
            int upper = this.Transitions.GetUpperIndex(this.StateStarts, fromState);
            return GetTransitionIndex(this.Transitions, symbol, this.Alphabet, this.ComparerField, lower, upper);
        }

        private void Minimize()
        {
            var registered = new Dictionary<StateSignature, int>(new StateSignatureEqualityComparer());
            var mergeList = this.GetMergeList();
            this.Register(this.RootTransition.StateIndex, registered, mergeList);

            var registeredArray = registered.ToArray();
            var oldToNewStates = Enumerable.Range(0, registered.Count)
                                           .ToDictionary(i => registeredArray[i].Value, i => i);
            this.Transitions = new QTransition[registered.Sum(r => r.Key.Transitions.Length)];
            this.StateStarts = new int[registered.Count];
            for (int i = 0, transIndex = 0; i < this.StateStarts.Length; i++)
            {
                var old = registeredArray[i];
                for (int j = 0; j < old.Key.Transitions.Length; j++)
                {
                    var oldTr = old.Key.Transitions[j];
                    this.Transitions[transIndex + j] = new QTransition(
                        oldTr.AlphabetIndex, oldToNewStates[oldTr.StateIndex], oldTr.IsFinal);
                }

                this.StateStarts[i] = transIndex;
                transIndex += old.Key.Transitions.Length;
            }

            this.RootTransition = new QTransition(
                0, oldToNewStates[this.RootTransition.StateIndex], this.RootTransition.IsFinal);
        }

        private int Register(int state, Dictionary<StateSignature, int> registered, MergeList mergeList)
        {
            int lower = this.StateStarts[state];
            int upper = this.Transitions.GetUpperIndex(this.StateStarts, state);
            int registeredState;

            for (int i = lower; i < upper; i++)
            {
                int sj = this.Transitions[i].StateIndex;
                registeredState = this.Register(sj, registered, mergeList);
                if (registeredState >= 0)
                {
                    // state sj has to be substituted with registeredState                    
                    foreach (var t in mergeList.GetTransitionIndexes(sj))
                    {
                        var transition = this.Transitions[t];
                        this.Transitions[t] = new QTransition(
                            transition.AlphabetIndex, registeredState, transition.IsFinal);
                    }

                    mergeList.Merge(registeredState, sj);
                }
            }

            var sigTransitions = new QTransition[upper - lower];
            for (int i = lower; i < upper; i++)
            {
                sigTransitions[i - lower] = this.Transitions[i];
            }

            var signature = new StateSignature(sigTransitions);
            if (registered.TryGetValue(signature, out registeredState))
            {
                return registeredState;
            }

            registered.Add(signature, state);
            return -1;
        }
    }
}