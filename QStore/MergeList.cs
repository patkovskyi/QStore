// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeList.cs" company="Dmytro Patkovskyi">
//   Dmytro Patkovskyi, released under MIT license
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace QStore
{
    using System.Collections.Generic;

    /// <summary>
    /// The purpose of this structure is to
    /// store indexes of transitions that point to
    /// each state in the automaton.
    /// Used in Revuz algorithm.
    /// </summary>
    public class MergeList
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
    }
}