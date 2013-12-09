namespace QStore.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using QStore.Core.Extensions;
    using QStore.Core.Interfaces;
    using QStore.Core.Structs;

    [DataContract]
    [Serializable]
    public class QIndexedSet : IIndexedSequenceSet<char>
    {
        [DataMember(Order = 2)]
        protected internal int[] PathsLeft;

        [DataMember(Order = 1)]
        protected internal QSet Set;

        public int Count
        {
            get
            {
                return this.Set.Count;
            }
        }

        public IComparer<char> Comparer
        {
            get
            {
                return this.Set.Comparer;
            }
        }

        public static QIndexedSet Create(IEnumerable<IEnumerable<char>> sequences, IComparer<char> comparer)
        {
            var set = QSet.Create(sequences, comparer);
            var indexedSet = new QIndexedSet { Set = set, PathsLeft = new int[set.Transitions.Length] };
            var pathsFromState = new int[set.StateStarts.Length];
            indexedSet.CountPaths(set.RootTransition.StateIndex, pathsFromState);
            set.Count += set.RootTransition.IsFinal ? 1 : 0;
            return indexedSet;
        }

        public bool Contains(IEnumerable<char> sequence)
        {
            return this.Set.Contains(sequence);
        }

        public IEnumerable<char[]> Enumerate()
        {
            return this.Set.Enumerate(this.Set.RootTransition);
        }

        public IEnumerable<char[]> EnumerateByPrefix(IEnumerable<char> prefix)
        {
            return this.Set.EnumerateByPrefix(prefix);
        }

        public IEnumerable<KeyValuePair<char[], int>> EnumerateByPrefixWithIndex(IEnumerable<char> prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            QSetTransition transition;
            var fromStack = new Stack<int>();
            if (this.Set.TrySendSequence(this.Set.RootTransition, prefix, out transition, fromStack))
            {
                int index =
                    fromStack.Select(i => this.PathsLeft[i - 1] + (this.Set.Transitions[i - 1].IsFinal ? 1 : 0)).Sum();
                if (fromStack.Count > 0)
                {
                    index += this.Set.RootTransition.IsFinal ? 1 : 0;
                    index -= transition.IsFinal ? 1 : 0;
                }

                return
                    this.Set.Enumerate(transition, fromStack).Select((s, i) => new KeyValuePair<char[], int>(s, i + index));
            }

            return Enumerable.Empty<KeyValuePair<char[], int>>();
        }

        public IEnumerable<KeyValuePair<char[], int>> EnumerateWithIndex()
        {
            return this.Set.Enumerate(this.Set.RootTransition).Select((s, i) => new KeyValuePair<char[], int>(s, i));
        }

        public char[] GetByIndex(int index)
        {
            this.ThrowIfIndexIsOutOfRange(index);
            var list = new List<char>();
            var nextTransition = this.Set.RootTransition;
            while (index > 0 || !nextTransition.IsFinal)
            {
                int lower = this.Set.StateStarts[nextTransition.StateIndex];
                int upper = this.Set.Transitions.GetUpperIndex(this.Set.StateStarts, nextTransition.StateIndex);
                index -= nextTransition.IsFinal ? 1 : 0;

                int nextTransitionIndex = Array.BinarySearch(this.PathsLeft, lower, upper - lower, index);
                if (nextTransitionIndex < 0)
                {
                    nextTransitionIndex = (~nextTransitionIndex) - 1;
                }

                index -= this.PathsLeft[nextTransitionIndex];

                nextTransition = this.Set.Transitions[nextTransitionIndex];
                list.Add(this.Set.Alphabet[nextTransition.AlphabetIndex]);
            }

            var result = new char[list.Count];
            list.CopyTo(result);
            return result;
        }

        public int GetIndex(IEnumerable<char> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            int currentState = this.Set.RootTransition.StateIndex;
            int pathsAfterThisChoice = this.Set.Count;
            int lexicographicIndex = this.Set.RootTransition.IsFinal ? 1 : 0;

            foreach (var element in sequence)
            {
                int upper = this.Set.Transitions.GetUpperIndex(this.Set.StateStarts, currentState);
                int transitionIndex;
                if (this.Set.TrySend(currentState, element, out transitionIndex))
                {
                    if (transitionIndex + 1 < upper)
                    {
                        pathsAfterThisChoice = this.PathsLeft[transitionIndex + 1];
                    }

                    var transition = this.Set.Transitions[transitionIndex];
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

        public void SetComparer(IComparer<char> comparer)
        {
            this.Set.SetComparer(comparer);
        }

        protected internal void ThrowIfIndexIsOutOfRange(int index)
        {
            if (index < 0 || index >= this.Set.Count)
            {
                throw new IndexOutOfRangeException(string.Format(ErrorMessages.IndexOutOfRange, index, this.Set.Count));
            }
        }

        private void CountPaths(int fromState, int[] pathsFromState)
        {
            int lower = this.Set.StateStarts[fromState];
            int upper = this.Set.Transitions.GetUpperIndex(this.Set.StateStarts, fromState);
            int pathsLeftCounter = 0;

            for (int i = lower; i < upper; i++)
            {
                this.PathsLeft[i] = pathsLeftCounter;
                pathsLeftCounter += this.Set.Transitions[i].IsFinal ? 1 : 0;
                int nextState = this.Set.Transitions[i].StateIndex;
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