namespace QStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    using QStore.Comparers;
    using QStore.Extensions;
    using QStore.Minimization;
    using QStore.Structs;

    [DataContract]
    [Serializable]
    public class QStringSet
    {
        [NonSerialized]
        protected internal IComparer<char> ComparerField;

        [DataMember(Order = 1)]
        protected internal int[] LowerBounds;

        [DataMember(Order = 2)]
        protected internal int RootState;

        [DataMember(Order = 3)]
        protected internal QTransition[] Transitions;

        protected QStringSet()
        {
        }

        public IComparer<char> Comparer
        {
            get
            {
                return this.ComparerField;
            }
        }

        [DataMember(Order = 4)]
        public int WordCount { get; protected internal set; }

        public static QStringSet Create(IEnumerable<string> words, IComparer<char> comparer)
        {
            return Create(new QStringSet(), words, comparer);
        }

        public bool Contains(IEnumerable<char> word)
        {
            if (word == null)
            {
                throw new ArgumentNullException("word");
            }

            int outState;
            if (this.TrySendWord(this.RootState, word, out outState))
            {
                return this.IsFinal(outState);
            }

            return false;
        }

        public IEnumerable<string> Enumerate()
        {
            return this.Enumerate(this.RootState, string.Empty);
        }

        public IEnumerable<string> EnumerateByPrefix(IEnumerable<char> prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            var prefixStr = prefix as string ?? new string(prefix.ToArray());
            int stateAfterPrefix;
            if (this.TrySendWord(this.RootState, prefixStr, out stateAfterPrefix))
            {
                return this.Enumerate(stateAfterPrefix, prefixStr);
            }

            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> Predecessors(IEnumerable<char> word)
        {
            throw new NotImplementedException();
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

        public IEnumerable<string> Successors(IEnumerable<char> word)
        {
            throw new NotImplementedException();
        }

        protected internal IEnumerable<string> Enumerate(int fromState, string prefix)
        {
            if (this.IsFinal(fromState))
            {
                yield return prefix;
            }

            var word = new StringBuilder(prefix);
            var lowerStack = new Stack<int>();
            var upperStack = new Stack<int>();

            int lower = this.LowerBound(fromState);
            int upper = this.UpperBound(fromState);

            lowerStack.Push(lower);
            upperStack.Push(upper);

            while (upperStack.Count > 0)
            {
                lower = lowerStack.Pop();
                upper = upperStack.Peek();

                if (lower < upper)
                {
                    QTransition fromTransition = this.Transitions[lower];
                    word.Append(fromTransition.Symbol);

                    int nextState = fromTransition.StateIndex;
                    if (this.IsFinal(nextState))
                    {
                        yield return word.ToString();
                    }

                    int nextLower = this.LowerBound(nextState);
                    int nextUpper = this.UpperBound(nextState);
                    lowerStack.Push(lower + 1);
                    lowerStack.Push(nextLower);
                    upperStack.Push(nextUpper);
                }
                else
                {
                    upperStack.Pop();
                    if (word.Length > 0)
                    {
                        word.Remove(word.Length - 1, 1);
                    }
                }
            }
        }

        protected internal bool TrySendSymbol(int inState, char symbol, out int outState)
        {
            int trIndex = this.GetTransitionIndex(inState, symbol);
            if (trIndex >= 0)
            {
                outState = this.Transitions[trIndex].StateIndex;
                return true;
            }

            outState = -1;
            return false;
        }

        protected internal bool TrySendWord(int inState, IEnumerable<char> word, out int outState)
        {
            outState = inState;

            foreach (var element in word)
            {
                if (!this.TrySendSymbol(inState, element, out outState))
                {
                    return false;
                }
            }

            return true;
        }

        protected static T Create<T>(T set, IEnumerable<string> words, IComparer<char> comparer) where T : QStringSet
        {
            if (words == null)
            {
                throw new ArgumentNullException("words");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            // outer list represents states, inner lists represent transitions from this state
            // State1 -------> State2 -------> ...... -------> StateN
            // |               |
            // a->State2       b->State3
            // |               |
            // d->State5       e->State4,final
            // |
            // z->StateN,final
            // capacity is set to alphabet.Length just to avoid few initial resizings            
            var transitions = new List<List<QTransition>> { new List<QTransition>() };
            var final = new List<bool> { false };
            int sequenceCounter = 0;
            foreach (var word in words)
            {
                int currentState = 0;
                foreach (var symbol in word)
                {
                    List<QTransition> currentStateTransitions = transitions[currentState];
                    int trIndex = currentStateTransitions.GetTransitionIndex(
                        symbol,
                        comparer,
                        0,
                        currentStateTransitions.Count);
                    if (trIndex >= 0)
                    {
                        // transition from currentState by symbol already exists
                        currentState = currentStateTransitions[trIndex].StateIndex;
                    }
                    else
                    {
                        // transition from currentState by symbol doesn't exist
                        trIndex = ~trIndex;

                        // add new state
                        currentState = transitions.Count;
                        transitions.Add(new List<QTransition>());
                        final.Add(false);

                        // add transition from current state to new state                                                
                        var newTransition = new QTransition(symbol, currentState);

                        // I know this Insert in the middle of the List<> doesn't look very clever,
                        // but I tried SortedDictionary and LinkedList approach: they are slower in practice
                        currentStateTransitions.Insert(trIndex, newTransition);
                    }
                }

                // throw exception if last state is already final (means duplicate word)
                if (final[currentState])
                {
                    throw new ArgumentException(string.Format(Messages.DuplicateKey, word));
                }

                // mark last state in this word as final
                final[currentState] = true;
                ++sequenceCounter;
            }

            set.ComparerField = comparer;
            set.RootState = 0;
            set.LowerBounds = new int[transitions.Count];
            set.Transitions = new QTransition[transitions.Sum(s => s.Count)];
            set.WordCount = sequenceCounter;

            for (int i = 0, transitionIndex = 0; i < transitions.Count; i++)
            {
                set.LowerBounds[i] = transitionIndex;
                for (int j = 0; j < transitions[i].Count; j++, transitionIndex++)
                {
                    set.Transitions[transitionIndex] = transitions[i][j];
                }

                if (final[i])
                {
                    set.MakeFinal(i);
                }
            }

            // set.Minimize();
            return set;
        }

        protected bool IsFinal(int state)
        {
            return (this.LowerBounds[state] & -2147483648) != 0;
        }

        protected int LowerBound(int state)
        {
            return this.LowerBounds[state] & 2147483647;
        }

        protected int UpperBound(int state)
        {
            return state + 1 < this.LowerBounds.Length
                ? this.LowerBounds[state + 1] & 2147483647
                : this.Transitions.Length;
        }

        private MergeList GetMergeList()
        {
            var merge = new MergeList(this.LowerBounds.Length, this.Transitions.Length);
            for (int i = 0; i < this.Transitions.Length; i++)
            {
                merge.Add(i, this.Transitions[i].StateIndex);
            }

            return merge;
        }

        /// <returns>Transition index or bitwise complement to the index where it should be inserted.</returns>
        private int GetTransitionIndex(int fromState, char symbol)
        {
            int lower = this.LowerBound(fromState);
            int upper = this.UpperBound(fromState);
            return this.Transitions.GetTransitionIndex(symbol, this.ComparerField, lower, upper);
        }

        private void MakeFinal(int state)
        {
            this.LowerBounds[state] |= -2147483648;
        }

        private void Minimize()
        {
            var registered = new Dictionary<StateSignature, int>(new StateSignatureEqualityComparer());
            var mergeList = this.GetMergeList();
            this.Register(this.RootState, registered, mergeList);

            var registeredArray = registered.ToArray();
            var oldToNewStates = Enumerable.Range(0, registered.Count)
                .ToDictionary(i => registeredArray[i].Value, i => i);
            this.Transitions = new QTransition[registered.Sum(r => r.Key.Transitions.Length)];
            this.LowerBounds = new int[registered.Count];
            for (int i = 0, transIndex = 0; i < this.LowerBounds.Length; i++)
            {
                var old = registeredArray[i];
                for (int j = 0; j < old.Key.Transitions.Length; j++)
                {
                    var oldTr = old.Key.Transitions[j];
                    this.Transitions[transIndex + j] = new QTransition(oldTr.Symbol, oldToNewStates[oldTr.StateIndex]);
                }

                this.LowerBounds[i] = transIndex;
                if (registeredArray[i].Key.IsFinal)
                {
                    this.MakeFinal(i);
                }
                transIndex += old.Key.Transitions.Length;
            }

            this.RootState = oldToNewStates[this.RootState];
        }

        private int Register(int state, Dictionary<StateSignature, int> registered, MergeList mergeList)
        {
            int lower = this.LowerBound(state);
            int upper = this.UpperBound(state);
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
                        this.Transitions[t] = new QTransition(transition.Symbol, registeredState);
                    }

                    mergeList.Merge(registeredState, sj);
                }
            }

            var sigTransitions = new QTransition[upper - lower];
            for (int i = lower; i < upper; i++)
            {
                sigTransitions[i - lower] = this.Transitions[i];
            }

            var signature = new StateSignature(sigTransitions, this.IsFinal(state));
            if (registered.TryGetValue(signature, out registeredState))
            {
                return registeredState;
            }

            registered.Add(signature, state);
            return -1;
        }
    }
}