using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Extensions;

namespace QSpell.Sequences
{
    /// <summary>
    /// The purpose of this structure is to
    /// store indexes of transitions that point to
    /// each state in the automaton.
    /// Used in Revuz algorithm.
    /// </summary>
    internal class MergeList
    {
        List<Int32>[] transitions;

        public MergeList(Int32 numberOfStates, Int32 numberOfTransitions)
        {
            Int32 ratio = 1 + numberOfTransitions / numberOfStates;
            transitions = new List<Int32>[numberOfStates];
            for (int i = 0; i < numberOfStates; i++)
            {
                transitions[i] = new List<Int32>(ratio);
            }
        }

        internal void Add(Int32 transition, Int32 toState)
        {
            transitions[toState].Add(transition);
        }

        internal void Merge(Int32 stateToKeep, Int32 stateToRemove)
        {
            //we never delete the registered state, so we don't really need to have it's info updated here
            //transitions[stateToKeep].UnionWith(transitions[stateToRemove]);
            transitions[stateToRemove] = null;
        }

        internal IEnumerable<Int32> GetTransitionIndexes(Int32 toState)
        {
            return transitions[toState];
        }
    }
}
