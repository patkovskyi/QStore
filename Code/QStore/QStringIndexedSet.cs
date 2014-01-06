namespace QStore
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class QStringIndexedSet : QStringSet
    {
        [DataMember(Order = 1)]
        protected internal int[] PathsLeft;

        public static new QStringIndexedSet Create(IEnumerable<string> sequences, IComparer<char> comparer)
        {
            var indexedSet = QStringSet.Create(new QStringIndexedSet(), sequences, comparer);
            indexedSet.PathsLeft = new int[indexedSet.Transitions.Length];
            var pathsFromState = new int[indexedSet.LowerBounds.Length];
            indexedSet.CountPaths(indexedSet.RootState, pathsFromState);            
            return indexedSet;
        }

        public string GetByIndex(int index)
        {
            this.ThrowIfIndexIsOutOfRange(index);
            var list = new List<char>();
            var state = this.RootState;
            while (index > 0 || !IsFinal(state))
            {
                int lower = LowerBound(state);
                int upper = UpperBound(state);
                index -= IsFinal(state) ? 1 : 0;

                int nextTransitionIndex = Array.BinarySearch(this.PathsLeft, lower, upper - lower, index);
                if (nextTransitionIndex < 0)
                {
                    nextTransitionIndex = (~nextTransitionIndex) - 1;
                }

                index -= this.PathsLeft[nextTransitionIndex];

                var transition = this.Transitions[nextTransitionIndex];
                state = transition.StateIndex;
                list.Add(transition.Symbol);
            }

            return new string(list.ToArray());
        }

        public int GetIndex(IEnumerable<char> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            int currentState = this.RootState;
            int pathsAfterThisChoice = this.WordCount;
            int lexicographicIndex = IsFinal(currentState) ? 1 : 0;

            foreach (var element in sequence)
            {
                int upper = UpperBound(currentState);
                int transitionIndex;
                if (this.TrySendSymbol(currentState, element, out transitionIndex))
                {
                    if (transitionIndex + 1 < upper)
                    {
                        pathsAfterThisChoice = this.PathsLeft[transitionIndex + 1];
                    }

                    var transition = this.Transitions[transitionIndex];
                    currentState = transition.StateIndex;
                    lexicographicIndex += IsFinal(currentState) ? 1 : 0;
                    lexicographicIndex += this.PathsLeft[transitionIndex];                    
                }
                else
                {
                    return ~transitionIndex < upper
                        ? ~(lexicographicIndex + this.PathsLeft[~transitionIndex])
                        : ~pathsAfterThisChoice;
                }
            }

            // we added one extra for last final transition
            return lexicographicIndex - 1;
        }

        protected internal void ThrowIfIndexIsOutOfRange(int index)
        {
            if (index < 0 || index >= this.WordCount)
            {
                throw new IndexOutOfRangeException(string.Format(Messages.IndexOutOfRange, index, this.WordCount));
            }
        }

        private void CountPaths(int fromState, int[] pathsFromState)
        {
            int lower = LowerBound(fromState);
            int upper = UpperBound(fromState);
            int pathsLeftCounter = 0;

            for (int i = lower; i < upper; i++)
            {
                this.PathsLeft[i] = pathsLeftCounter;
                pathsLeftCounter += IsFinal(fromState) ? 1 : 0;
                int nextState = this.Transitions[i].StateIndex;
                if (pathsFromState[nextState] == 0)
                {
                    this.CountPaths(nextState, pathsFromState);
                }

                pathsLeftCounter += pathsFromState[nextState];
            }

            pathsFromState[fromState] = pathsLeftCounter;
        }
    }
}