namespace QStore
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using QStore.Extensions;

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
            indexedSet.CountPaths(indexedSet.RootTransition.StateIndex, pathsFromState);
            indexedSet.WordCount += indexedSet.RootTransition.IsFinal ? 1 : 0;
            return indexedSet;
        }

        public string GetByIndex(int index)
        {
            this.ThrowIfIndexIsOutOfRange(index);
            var list = new List<char>();
            var nextTransition = this.RootTransition;
            while (index > 0 || !nextTransition.IsFinal)
            {
                int lower = this.LowerBounds[nextTransition.StateIndex];
                int upper = this.Transitions.GetUpperBound(this.LowerBounds, nextTransition.StateIndex);
                index -= nextTransition.IsFinal ? 1 : 0;

                int nextTransitionIndex = Array.BinarySearch(this.PathsLeft, lower, upper - lower, index);
                if (nextTransitionIndex < 0)
                {
                    nextTransitionIndex = (~nextTransitionIndex) - 1;
                }

                index -= this.PathsLeft[nextTransitionIndex];

                nextTransition = this.Transitions[nextTransitionIndex];
                list.Add(nextTransition.Symbol);
            }

            return new string(list.ToArray());
        }

        public int GetIndex(IEnumerable<char> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            int currentState = this.RootTransition.StateIndex;
            int pathsAfterThisChoice = this.WordCount;
            int lexicographicIndex = this.RootTransition.IsFinal ? 1 : 0;

            foreach (var element in sequence)
            {
                int upper = this.Transitions.GetUpperBound(this.LowerBounds, currentState);
                int transitionIndex;
                if (this.TrySendSymbol(currentState, element, out transitionIndex))
                {
                    if (transitionIndex + 1 < upper)
                    {
                        pathsAfterThisChoice = this.PathsLeft[transitionIndex + 1];
                    }

                    var transition = this.Transitions[transitionIndex];
                    lexicographicIndex += transition.IsFinal ? 1 : 0;
                    lexicographicIndex += this.PathsLeft[transitionIndex];
                    currentState = transition.StateIndex;
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
            int lower = this.LowerBounds[fromState];
            int upper = this.Transitions.GetUpperBound(this.LowerBounds, fromState);
            int pathsLeftCounter = 0;

            for (int i = lower; i < upper; i++)
            {
                this.PathsLeft[i] = pathsLeftCounter;
                pathsLeftCounter += this.Transitions[i].IsFinal ? 1 : 0;
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