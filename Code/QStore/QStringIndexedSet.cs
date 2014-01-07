namespace QStore
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

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
            var sb = new StringBuilder();
            var state = this.RootState;
            while (index > 0 || !this.IsFinal(state))
            {
                int lower = this.LowerBound(state);
                int upper = this.UpperBound(state);

                int nextTransitionIndex = Array.BinarySearch(this.PathsLeft, lower, upper - lower, index);
                if (nextTransitionIndex < 0)
                {
                    nextTransitionIndex = ~nextTransitionIndex - 1;
                }

                index -= this.PathsLeft[nextTransitionIndex];

                var transition = this.Transitions[nextTransitionIndex];
                state = transition.StateIndex;
                sb.Append(transition.Symbol);
            }

            return sb.ToString();
        }

        // TODO: think of simplifying this method
        public int GetIndex(IEnumerable<char> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            int currentState = this.RootState;
            int pathsAfterThisChoice = this.WordCount;
            int lexicographicIndex = 0;

            foreach (var element in sequence)
            {
                int upper = this.UpperBound(currentState);
                int nextTransition;
                if (this.TrySendSymbol(currentState, element, out nextTransition))
                {
                    if (nextTransition + 1 < upper)
                    {
                        pathsAfterThisChoice = this.PathsLeft[nextTransition + 1];
                    }

                    var transition = this.Transitions[nextTransition];
                    currentState = transition.StateIndex;
                    lexicographicIndex += this.PathsLeft[nextTransition];
                }
                else
                {
                    return ~nextTransition < upper
                        ? ~(lexicographicIndex + this.PathsLeft[~nextTransition])
                        : ~pathsAfterThisChoice;
                }
            }

            return this.IsFinal(currentState) ? lexicographicIndex : lexicographicIndex - 1;
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
            int lower = this.LowerBound(fromState);
            int upper = this.UpperBound(fromState);
            int pathsLeftCounter = this.IsFinal(fromState) ? 1 : 0;

            for (int i = lower; i < upper; i++)
            {
                this.PathsLeft[i] = pathsLeftCounter;
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