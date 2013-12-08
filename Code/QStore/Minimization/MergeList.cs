namespace QStore.Minimization
{
    using System.Collections.Generic;

    /// <summary>
    /// The purpose of this structure is to
    /// store indexes of transitions that point to
    /// each state in the automaton.
    /// Used in Revuz minimization algorithm.
    /// </summary>
    internal class MergeList
    {
        private readonly List<int>[] transitions;

        public MergeList(int numberOfStates, int numberOfTransitions)
        {
            int ratio = 1 + (numberOfTransitions / numberOfStates);
            this.transitions = new List<int>[numberOfStates];
            for (int i = 0; i < numberOfStates; i++)
            {
                this.transitions[i] = new List<int>(ratio);
            }
        }

        internal void Add(int transition, int toState)
        {
            this.transitions[toState].Add(transition);
        }

        internal List<int> GetTransitionIndexes(int toState)
        {
            return this.transitions[toState];
        }

        internal void Merge(int stateToKeep, int stateToRemove)
        {
            this.transitions[stateToRemove] = null;
        }
    }
}