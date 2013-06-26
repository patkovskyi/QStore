namespace QStore.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using QStore.Core.Extensions;
    using QStore.Core.Interfaces;
    using QStore.Core.Structs;

    public class QIndexedSet<T> : QSet<T>, IIndexedSequenceSet<T>
    {
        protected internal int[] PathsLeft;

        public static new QIndexedSet<T> Create(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer)
        {
            return QIndexedSet<T>.Create<QIndexedSet<T>>(sequences, comparer);
        }

        public List<T> GetByIndex(long index)
        {
            this.ThrowIfIndexIsOutOfRange(index);
            var result = new List<T>();
            var nextTransition = new QSetTransition(0, this.RootState, false);
            while (index > 0 || !nextTransition.IsFinal)
            {
                int lower = this.StateStarts[nextTransition.StateIndex];
                int upper = this.Transitions.GetUpperIndex(this.StateStarts, nextTransition.StateIndex);

                // TODO: fix this (int) cast
                int nextTransitionIndex = Array.BinarySearch(this.PathsLeft, lower, upper - lower, (int)index);
                if (nextTransitionIndex < 0)
                {
                    nextTransitionIndex = (~nextTransitionIndex) - 1;
                }

                index -= nextTransition.IsFinal ? 1 : 0;
                index -= this.PathsLeft[nextTransitionIndex];

                nextTransition = this.Transitions[nextTransitionIndex];
                result.Add(this.Alphabet[nextTransition.AlphabetIndex]);
            }

            return result;
        }

        public IEnumerable<KeyValuePair<T[], long>> GetByPrefixWithIndex(IEnumerable<T> prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            QSetTransition transition;
            var fromStack = new Stack<int>();
            if (this.TrySendSequence(this.RootState, prefix, out transition, fromStack))
            {
                long index =
                    fromStack.Select(i => this.PathsLeft[i - 1] + (this.Transitions[i - 1].IsFinal ? 1 : 0)).Sum();
                return
                    this.Enumerate(transition.StateIndex, fromStack)
                        .Select((s, i) => new KeyValuePair<T[], long>(s, i + index));
            }

            return Enumerable.Empty<KeyValuePair<T[], long>>();
        }

        public long GetIndex(IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            int currentState = this.RootState;
            long pathsAfterThisChoice = this.Count;
            long lexicographicIndex = 0;

            foreach (var element in sequence)
            {
                int upper = this.Transitions.GetUpperIndex(this.StateStarts, currentState);
                int transitionIndex;
                if (this.TrySend(currentState, element, out transitionIndex))
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

            if (currentState == this.RootState)
            {
                throw new ArgumentException(ErrorMessages.EmptySequencesAreNotSupported);
            }

            // we added one extra for last final transition
            return lexicographicIndex - 1;
        }

        protected static new TIndexedSet Create<TIndexedSet>(
            IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer) where TIndexedSet : QIndexedSet<T>, new()
        {
            var indexedSet = QSet<T>.Create<TIndexedSet>(sequences, comparer);
            indexedSet.PathsLeft = new int[indexedSet.Transitions.Length];
            var pathsFromState = new int[indexedSet.StateStarts.Length];
            indexedSet.CountPaths(indexedSet.RootState, pathsFromState);
            return indexedSet;
        }

        protected void ThrowIfIndexIsOutOfRange(long index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new IndexOutOfRangeException(string.Format(ErrorMessages.IndexOutOfRange, index, this.Count));
            }
        }

        private void CountPaths(int fromState, int[] pathsFromState)
        {
            int lower = this.StateStarts[fromState];
            int upper = this.Transitions.GetUpperIndex(this.StateStarts, fromState);
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