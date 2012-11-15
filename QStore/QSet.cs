namespace QStore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Comparers;
    using QStore.Extensions;

    /// <summary>
    /// Absolute memory minimum. 
    /// Transition function in 
    /// worst-case: O(log(alphabetSize)) 
    /// average case: O(log(branchingFactor))
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class QSet<T> : IEnumerable<IEnumerable<T>>
    {
        #region Fields

        protected T[] alphabet;

        protected IList<int> lowerTransitionIndexes;

        protected int start;

        /// <summary>
        /// TODO: think of serialization.
        /// </summary>
        protected IComparer<T> symbolComparer;

        protected IList<QSetTransition> transitions;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="sequences">
        /// The sequences.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <param name="minimize">
        /// The minimize.
        /// </param>
        /// <returns>
        /// The QSet.
        /// </returns>                       
        public static QSet<T> Create(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer, bool minimize)
        {
            var enumeratedSequences = sequences as IEnumerable<T>[] ?? sequences.ToArray();
            return Create<QSet<T>>(enumeratedSequences, enumeratedSequences.SelectMany(s => s), comparer, minimize);
        }

        /// <summary>
        /// The contains sequence.
        /// </summary>
        /// <param name="sequence">
        /// The sequence.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ContainsSequence(IEnumerable<T> sequence)
        {
            List<int> transitionPath;
            return this.ContainsSequence(sequence, out transitionPath);
        }

        /// <summary>
        /// The get by prefix.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<T[]> GetByPrefix(IEnumerable<T> prefix)
        {
            int curState = this.start;
            var enumerated = new List<T>();
            foreach (var element in prefix)
            {
                int trIndex = this.GetTransitionIndex(curState, element);
                if (trIndex >= 0) curState = this.transitions[trIndex].StateIndex;
                else return Enumerable.Empty<T[]>();
                enumerated.Add(element);
            }

            return this.Enumerate(curState).Select(sequence => enumerated.Concat(sequence).ToArray());
        }

        /// <summary>
        /// The get states count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetStatesCount()
        {
            return this.lowerTransitionIndexes.Count;
        }

        /// <summary>
        /// The get transitions count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetTransitionsCount()
        {
            return this.transitions.Count;
        }

        /// <summary>
        /// The minimize.
        /// </summary>
        public virtual void Minimize()
        {
            this.RevuzMinimize();
        }

        #endregion

        #region IEnumerable<IEnumerable<T>> Members

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return this.Enumerate(this.start).GetEnumerator();
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="sequences">
        /// The sequences.
        /// </param>
        /// <param name="alphabet">
        /// The alphabet.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <param name="minimize">
        /// The minimize.
        /// </param>
        /// <typeparam name="S">
        /// </typeparam>
        /// <returns>
        /// The <see cref="S"/>.
        /// </returns>
        internal static S Create<S>(IEnumerable<IEnumerable<T>> sequences, IEnumerable<T> alphabet, IComparer<T> comparer, bool minimize)
            where S : QSet<T>, new()
        {
            T[] orderedAlphabet = alphabet.Distinct().OrderBy(a => a, comparer).ToArray();
            var alphabetDict = orderedAlphabet.Select((c, i) => new Tuple<T, int>(c, i)).ToDictionary(
                t => t.Item1, t => t.Item2);

            var transitions = new List<List<QSetTransition>>(orderedAlphabet.Length) { new List<QSetTransition>() };
            foreach (var sequence in sequences)
            {
                int curState = -1, nextState = 0, transitionIndex = -1;
                foreach (var element in sequence)
                {
                    curState = nextState;
                    transitionIndex = GetTransitionIndex(
                        transitions[curState], element, orderedAlphabet, comparer, 0, transitions[curState].Count);
                    if (transitionIndex < 0)
                    {
                        transitionIndex = ~transitionIndex;

                        // Int32 alphabetIndex = alphabet.OptimalSearch(element);
                        int alphabetIndex = alphabetDict[element];

                        // add new state
                        nextState = transitions.Count;
                        transitions.Add(new List<QSetTransition>());

                        // add transition from current state to new state                                                
                        var newTransition = new QSetTransition(alphabetIndex, nextState, false);
                        transitions[curState].Insert(transitionIndex, newTransition);
                    }
                    else nextState = transitions[curState][transitionIndex].StateIndex;
                }

                var tmp = transitions[curState][transitionIndex];
                transitions[curState][transitionIndex] = new QSetTransition(tmp.AlphabetIndex, tmp.StateIndex, true);
            }

            var result = new S
            {
                symbolComparer = comparer, 
                alphabet = orderedAlphabet, 
                start = 0, 
                lowerTransitionIndexes = new int[transitions.Count], 
                transitions = new QSetTransition[transitions.Sum(s => s.Count)]
            };

            for (int i = 0, trIndex = 0; i < transitions.Count; i++)
            {
                result.lowerTransitionIndexes[i] = trIndex;
                for (int j = 0; j < transitions[i].Count; j++, trIndex++)
                {
                    result.transitions[trIndex] = transitions[i][j];
                }
            }

            if (minimize) result.Minimize();

            return result;
        }

        /// <summary>
        /// The try send.
        /// </summary>
        /// <param name="fromState">
        /// The from state.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="transition">
        /// The transition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool TrySend(int fromState, T input, out QSetTransition transition)
        {
            int transitionIndex = this.GetTransitionIndex(fromState, input);
            if (transitionIndex >= 0)
            {
                transition = this.transitions[transitionIndex];
                return true;
            }
            else
            {
                transition = default(QSetTransition);
                return false;
            }
        }

        /// <summary>
        /// The try send.
        /// </summary>
        /// <param name="fromState">
        /// The from state.
        /// </param>
        /// <param name="inputSequence">
        /// The input sequence.
        /// </param>
        /// <param name="transition">
        /// The transition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool TrySend(int fromState, IEnumerable<T> inputSequence, out QSetTransition transition)
        {
            int currentState = fromState;
            transition = default(QSetTransition);
            foreach (var input in inputSequence)
            {
                if (this.TrySend(currentState, input, out transition)) currentState = transition.StateIndex;
                else return false;
            }

            return true;
        }

        /// <summary>
        /// The contains sequence.
        /// </summary>
        /// <param name="sequence">
        /// The sequence.
        /// </param>
        /// <param name="transitionPath">
        /// The transition path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool ContainsSequence(IEnumerable<T> sequence, out List<int> transitionPath)
        {
            transitionPath = new List<int>();

            if (!sequence.Any()) return false;

            int curState = this.start, trIndex = -1;
            foreach (var element in sequence)
            {
                trIndex = this.GetTransitionIndex(curState, element);
                if (trIndex >= 0)
                {
                    transitionPath.Add(trIndex);
                    curState = this.transitions[trIndex].StateIndex;
                }
                else return false;
            }

            return this.transitions[trIndex].Final;
        }

        /// <summary>
        /// The enumerate.
        /// </summary>
        /// <param name="fromState">
        /// The from state.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected IEnumerable<IEnumerable<T>> Enumerate(int fromState)
        {
            int lower = this.lowerTransitionIndexes[fromState];
            int upper = this.transitions.GetUpperIndex(this.lowerTransitionIndexes, fromState);

            for (int i = lower; i < upper; i++)
            {
                var tr = this.transitions[i];
                T element = this.alphabet[tr.AlphabetIndex];
                if (tr.Final) yield return new[] { element };

                foreach (var w in this.Enumerate(tr.StateIndex).Select(w => w.Prepend(element)))
                {
                    yield return w;
                }
            }
        }

        /// <summary>
        /// The get alphabet index.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected int GetAlphabetIndex(T item)
        {
            return this.alphabet.BinarySearch(item, this.symbolComparer);
        }

        /// <summary>
        /// The get transition index.
        /// </summary>
        /// <param name="fromState">
        /// The from State.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// Transition index or bitwise complement to the index where it should be inserted.
        /// </returns>
        protected int GetTransitionIndex(int fromState, T input)
        {
            int lower = this.lowerTransitionIndexes[fromState];
            int upper = this.transitions.GetUpperIndex(this.lowerTransitionIndexes, fromState);
            return GetTransitionIndex(this.transitions, input, this.alphabet, this.symbolComparer, lower, upper);
        }

        /// <summary>
        /// Acyclyc DFA minimization algorithm, O(L) time?
        /// </summary>
        protected void RevuzMinimize()
        {
            var registered = new Dictionary<StateSignature, int>(new StateSignatureEqualityComparer());
            var mergeList = this.GetMergeList();
            this.Register(this.start, registered, mergeList);

            var registeredArray = registered.ToArray();
            var oldToNewStates = Enumerable.Range(0, registered.Count).ToDictionary(
                i => registeredArray[i].Value, i => i);
            this.transitions = new QSetTransition[registered.Sum(r => r.Key.Transitions.Length)];
            this.lowerTransitionIndexes = new int[registered.Count];
            for (int i = 0, transIndex = 0; i < this.lowerTransitionIndexes.Count; i++)
            {
                // TODO: this could be optimized
                var old = registeredArray[i];
                for (int j = 0; j < old.Key.Transitions.Length; j++)
                {
                    var oldTr = old.Key.Transitions[j];
                    this.transitions[transIndex + j] = new QSetTransition(
                        oldTr.AlphabetIndex, oldToNewStates[oldTr.StateIndex], oldTr.Final);
                }

                this.lowerTransitionIndexes[i] = transIndex;
                transIndex += old.Key.Transitions.Length;
            }

            this.start = oldToNewStates[this.start];
        }

        /// <summary>
        /// The get transition index.
        /// </summary>
        /// <param name="transitions">
        /// The transitions.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="alphabet">
        /// The alphabet.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <param name="lower">
        /// The lower.
        /// </param>
        /// <param name="upper">
        /// The upper.
        /// </param>
        /// <returns>
        /// Transition index or bitwise complement to the index where it should be inserted.
        /// </returns>
        private static int GetTransitionIndex(
            IList<QSetTransition> transitions, T input, T[] alphabet, IComparer<T> comparer, int lower, int upper)
        {
            const int BINARY_SEARCH_THRESHOLD = 5;
            if (upper - lower >= BINARY_SEARCH_THRESHOLD)
            {
                // Binary search
                upper--;
                while (lower <= upper)
                {
                    int middle = (lower + upper) / 2;
                    int comparisonResult = comparer.Compare(alphabet[transitions[middle].AlphabetIndex], input);
                    if (comparisonResult == 0) return middle;
                    else if (comparisonResult > 0) upper = middle - 1;
                    else lower = middle + 1;
                }

                return ~lower;
            }
            else
            {
                // Linear search
                for (int i = lower; i < upper; i++)
                {
                    int comp = comparer.Compare(alphabet[transitions[i].AlphabetIndex], input);
                    if (comp == 0) return i;
                    else if (comp > 0) return ~i;
                }

                return ~upper;
            }
        }

        /// <summary>
        /// The get merge list.
        /// </summary>
        /// <returns>
        /// The <see cref="MergeList"/>.
        /// </returns>
        private MergeList GetMergeList()
        {
            var merge = new MergeList(this.lowerTransitionIndexes.Count, this.transitions.Count);
            for (int i = 0; i < this.transitions.Count; i++)
            {
                merge.Add(i, this.transitions[i].StateIndex);
            }

            return merge;
        }

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="registered">
        /// The registered.
        /// </param>
        /// <param name="mergeList">
        /// The merge list.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int Register(int state, Dictionary<StateSignature, int> registered, MergeList mergeList)
        {
            int lower = this.lowerTransitionIndexes[state];
            int upper = this.transitions.GetUpperIndex(this.lowerTransitionIndexes, state);
            int registeredState;

            for (int i = lower; i < upper; i++)
            {
                int sj = this.transitions[i].StateIndex;
                registeredState = this.Register(sj, registered, mergeList);
                if (registeredState >= 0)
                {
                    // state sj has to be substituted with registeredState                    
                    foreach (var t in mergeList.GetTransitionIndexes(sj))
                    {
                        var transition = this.transitions[t];
                        this.transitions[t] = new QSetTransition(
                            transition.AlphabetIndex, registeredState, transition.Final);
                    }

                    mergeList.Merge(registeredState, sj);
                }
            }

            var sigTransitions = new QSetTransition[upper - lower];
            for (int i = lower; i < upper; i++)
            {
                sigTransitions[i - lower] = this.transitions[i];
            }

            var signature = new StateSignature(sigTransitions);
            if (registered.TryGetValue(signature, out registeredState)) return registeredState;
            else registered.Add(signature, state);

            return -1;
        }

        #endregion
    }
}