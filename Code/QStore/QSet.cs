namespace QStore
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using QStore.Minimization;

    [XmlType]
    public class QSet
    {
        [XmlElement(Order = 1)]
        internal QTransition RootTransition;

        [XmlElement(Order = 2)]
        internal int[] StateStarts;

        [XmlElement(Order = 3)]
        internal QTransition[] Transitions;

        public IComparer<char> Comparer
        {
            get
            {
                return Comparer<char>.Default;
            }
        }

        [XmlElement(Order = 4)]
        private int Count { get; set; }

        public static QSet Create(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            var comparer = Comparer<char>.Default;

            // outer list represents states, inner lists represent transitions from this state
            // State1 -------> State2 -------> ...... -------> StateN
            // |               |
            // a->State2       b->State3
            // |               |
            // d->State5       e->State4,final
            // |
            // z->StateN,final            
            var rootTransition = default(QTransition);
            var transitions = new List<List<QTransition>> { new List<QTransition>() };
            int keyCounter = 0;
            int transitionCounter = 0;

            foreach (var key in keys)
            {
                int nextState = 0, transitionIndex = -1;
                List<QTransition> currentStateTransitions = null;
                foreach (var symbol in key)
                {
                    currentStateTransitions = transitions[nextState];

                    transitionIndex = GetTransitionIndex(
                        currentStateTransitions,
                        symbol,
                        comparer,
                        0,
                        currentStateTransitions.Count);

                    if (transitionIndex >= 0)
                    {
                        // transition from currentState by symbol already exists
                        nextState = currentStateTransitions[transitionIndex].NextState;
                    }
                    else
                    {
                        // transition from currentState by symbol doesn't exist
                        transitionIndex = ~transitionIndex;

                        // add new state
                        nextState = transitions.Count;
                        transitions.Add(new List<QTransition>());
                        ++transitionCounter;

                        // add transition from current state to new state                                                
                        var newTransition = new QTransition(symbol, nextState, false);

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
                        throw new ArgumentException(string.Format(Messages.DuplicateKey, key));
                    }

                    currentStateTransitions[transitionIndex] = currentStateTransitions[transitionIndex].MakeFinal();
                }

                ++keyCounter;
            }

            var result = new QSet
            {
                RootTransition = rootTransition,
                StateStarts = new int[transitions.Count],
                Transitions = new QTransition[transitionCounter],
                Count = keyCounter
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
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Get()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetByPrefix(IEnumerable<char> keyPrefix)
        {
            throw new NotImplementedException();
        }

        private static int GetTransitionIndex(
            IList<QTransition> transitions,
            char symbol,
            IComparer<char> comparer,
            int lower,
            int upper)
        {
            const int BinarySearchThreshold = 1;
            if (upper - lower >= BinarySearchThreshold)
            {
                // binary search
                upper--;
                while (lower <= upper)
                {
                    int middle = lower + ((upper - lower) >> 1);
                    int comparisonResult = comparer.Compare(transitions[middle].Symbol, symbol);
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
                int comp = comparer.Compare(transitions[i].Symbol, symbol);
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
                merge.Add(i, this.Transitions[i].NextState);
            }

            return merge;
        }

        private void Minimize()
        {
            var registered = new Dictionary<StateSignature, int>(new StateSignatureEqualityComparer());
            var mergeList = this.GetMergeList();
            this.Register(this.RootTransition.NextState, registered, mergeList);
            
            var registeredArray = registered.ToArray();
            var oldToNewStates = Enumerable.Range(0, registered.Count)
                                           .ToDictionary(i => registeredArray[i].Value, i => i);
            this.Transitions = new QSetTransition[registered.Sum(r => r.Key.Transitions.Length)];
            this.StateStarts = new int[registered.Count];
            for (int i = 0, transIndex = 0; i < this.StateStarts.Length; i++)
            {
                var old = registeredArray[i];
                for (int j = 0; j < old.Key.Transitions.Length; j++)
                {
                    var oldTr = old.Key.Transitions[j];
                    this.Transitions[transIndex + j] = new QSetTransition(
                        oldTr.AlphabetIndex, oldToNewStates[oldTr.StateIndex], oldTr.IsFinal);
                }

                this.StateStarts[i] = transIndex;
                transIndex += old.Key.Transitions.Length;
            }

            this.RootTransition = new QSetTransition(
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
                        this.Transitions[t] = new QSetTransition(
                            transition.AlphabetIndex, registeredState, transition.IsFinal);
                    }

                    mergeList.Merge(registeredState, sj);
                }
            }

            var sigTransitions = new QSetTransition[upper - lower];
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