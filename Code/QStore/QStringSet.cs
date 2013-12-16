namespace QStore
{
    using System;
    using System.Collections.Generic;
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
        [NonSerialized]
        protected internal IComparer<char> ComparerField;

        [DataMember(Order = 2)]
        protected internal int[] LowerBounds;

        [DataMember(Order = 1)]
        protected internal QTransition RootTransition;

        [DataMember(Order = 3)]
        protected internal QTransition[] Transitions;

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
            var rootTransition = default(QTransition);
            var transitions = new List<List<QTransition>> { new List<QTransition>() };

            int sequenceCounter = 0;
            foreach (var word in words)
            {
                int nextState = 0, transitionIndex = -1;
                List<QTransition> currentStateTransitions = null;
                foreach (var symbol in word)
                {
                    currentStateTransitions = transitions[nextState];
                    transitionIndex = currentStateTransitions.GetTransitionIndex(
                        symbol,
                        comparer,
                        0,
                        currentStateTransitions.Count);
                    if (transitionIndex >= 0)
                    {
                        // transition from currentState by symbol already exists
                        nextState = currentStateTransitions[transitionIndex].StateIndex;
                    }
                    else
                    {
                        // transition from currentState by symbol doesn't exist
                        transitionIndex = ~transitionIndex;

                        // add new state
                        nextState = transitions.Count;
                        transitions.Add(new List<QTransition>());

                        // add transition from current state to new state                                                
                        var newTransition = new QTransition(symbol, nextState, false);

                        // I know this Insert in the middle of the List<> doesn't look very clever,
                        // but I tried SortedDictionary and LinkedList approach: they are slower in practice
                        currentStateTransitions.Insert(transitionIndex, newTransition);
                    }
                }

                // mark last transition in this word as final
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
                        throw new ArgumentException(string.Format(Messages.DuplicateKey, word));
                    }

                    currentStateTransitions[transitionIndex] = currentStateTransitions[transitionIndex].MakeFinal();
                }

                ++sequenceCounter;
            }

            var result = new QStringSet
            {
                ComparerField = comparer,
                RootTransition = rootTransition,
                LowerBounds = new int[transitions.Count],
                Transitions = new QTransition[transitions.Sum(s => s.Count)],
                WordCount = sequenceCounter
            };

            for (int i = 0, transitionIndex = 0; i < transitions.Count; i++)
            {
                result.LowerBounds[i] = transitionIndex;
                for (int j = 0; j < transitions[i].Count; j++, transitionIndex++)
                {
                    result.Transitions[transitionIndex] = transitions[i][j];
                }
            }

            result.Minimize();
            return result;
        }

        public bool Contains(IEnumerable<char> word)
        {
            if (word == null)
            {
                throw new ArgumentNullException("word");
            }

            QTransition transition;
            if (this.TrySendWord(this.RootTransition, word, out transition))
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

            QTransition transitionAfterPrefix;
            var prefixTransitionIndexChain = new Stack<int>();
            if (this.TrySendWord(this.RootTransition, prefix, out transitionAfterPrefix, prefixTransitionIndexChain))
            {
                return this.Enumerate(transitionAfterPrefix, prefixTransitionIndexChain);
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
                        : new string(fromStack.Reverse().Select(i => this.Transitions[i - 1].Symbol).ToArray());
            }

            fromStack = fromStack ?? new Stack<int>();
            var toStack = new Stack<int>();
            int lower = this.LowerBounds[fromTransition.StateIndex];
            int upper = this.Transitions.GetUpperBound(this.LowerBounds, fromTransition.StateIndex);
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
                            res[i] = this.Transitions[tmp[res.Length - i - 1] - 1].Symbol;
                        }

                        yield return new string(res);
                    }

                    int nextState = this.Transitions[lower].StateIndex;
                    int nextLower = this.LowerBounds[nextState];
                    int nextUpper = this.Transitions.GetUpperBound(this.LowerBounds, nextState);
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

        protected internal bool TrySend(int fromState, char symbol, out int outTransitionIndex)
        {
            outTransitionIndex = this.GetTransitionIndex(fromState, symbol);
            if (outTransitionIndex >= 0)
            {
                return true;
            }

            return false;
        }

        protected internal bool TrySendWord(
            QTransition inTransition,
            IEnumerable<char> word,
            out QTransition outTransition,
            Stack<int> transitionIndexChain = null)
        {
            if (word == null)
            {
                throw new ArgumentNullException("word");
            }

            outTransition = inTransition;

            foreach (var element in word)
            {
                int transitionIndex;
                if (this.TrySend(outTransition.StateIndex, element, out transitionIndex))
                {
                    outTransition = this.Transitions[transitionIndex];

                    if (transitionIndexChain != null)
                    {
                        transitionIndexChain.Push(transitionIndex + 1);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
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
            int lower = this.LowerBounds[fromState];
            int upper = this.Transitions.GetUpperBound(this.LowerBounds, fromState);
            return this.Transitions.GetTransitionIndex(symbol, this.ComparerField, lower, upper);
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
            this.LowerBounds = new int[registered.Count];
            for (int i = 0, transIndex = 0; i < this.LowerBounds.Length; i++)
            {
                var old = registeredArray[i];
                for (int j = 0; j < old.Key.Transitions.Length; j++)
                {
                    var oldTr = old.Key.Transitions[j];
                    this.Transitions[transIndex + j] = new QTransition(
                        oldTr.Symbol,
                        oldToNewStates[oldTr.StateIndex],
                        oldTr.IsFinal);
                }

                this.LowerBounds[i] = transIndex;
                transIndex += old.Key.Transitions.Length;
            }

            this.RootTransition = new QTransition(
                '\0',
                oldToNewStates[this.RootTransition.StateIndex],
                this.RootTransition.IsFinal);
        }

        private int Register(int state, Dictionary<StateSignature, int> registered, MergeList mergeList)
        {
            int lower = this.LowerBounds[state];
            int upper = this.Transitions.GetUpperBound(this.LowerBounds, state);
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
                        this.Transitions[t] = new QTransition(transition.Symbol, registeredState, transition.IsFinal);
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