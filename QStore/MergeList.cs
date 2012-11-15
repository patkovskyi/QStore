namespace QStore
{
    using System.Collections.Generic;

    /// <summary>
    /// The purpose of this structure is to
    /// store indexes of transitions that point to
    /// each state in the automaton.
    /// Used in Revuz algorithm.
    /// </summary>
    internal class MergeList
    {
        #region Fields

        private readonly List<int>[] transitions;

        #endregion

        #region Constructors and Destructors

        public MergeList(int numberOfStates, int numberOfTransitions)
        {
            int ratio = 1 + (numberOfTransitions / numberOfStates);
            this.transitions = new List<int>[numberOfStates];
            for (int i = 0; i < numberOfStates; i++)
            {
                this.transitions[i] = new List<int>(ratio);
            }
        }

        #endregion

        #region Methods

        internal void Add(int transition, int toState)
        {
            this.transitions[toState].Add(transition);
        }

        internal IEnumerable<int> GetTransitionIndexes(int toState)
        {
            return this.transitions[toState];
        }

        internal void Merge(int stateToKeep, int stateToRemove)
        {
            // we never delete the registered state, so we don't really need to have it's info updated here
            // transitions[stateToKeep].UnionWith(transitions[stateToRemove]);
            this.transitions[stateToRemove] = null;
        }

        #endregion
    }
}