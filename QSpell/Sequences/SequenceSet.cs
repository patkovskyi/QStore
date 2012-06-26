﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Security;
using QSpell.Extensions;

namespace QSpell.Sequences
{
    /// <summary>
    /// Absolute memory minimum. 
    /// Transition function in 
    /// worst-case: O(log(alphabetSize)) 
    /// average case: O(log(branchingFactor))
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SequenceSet<T> : IEnumerable<IEnumerable<T>>
    {
        #region CONSTRUCTORS
        public static SequenceSet<T> Create(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer, bool minimize)
        {
            T[] alphabet = sequences.SelectMany(s => s).Distinct().OrderBy(a => a, comparer).ToArray();
            var transitions = new List<List<SequenceSetTransition>>(alphabet.Length) { new List<SequenceSetTransition>() };
            foreach (var sequence in sequences)
            {
                Int32 curState = -1, nextState = 0, transitionIndex = -1;
                foreach (var element in sequence)
                {
                    curState = nextState;
                    transitionIndex = GetTransitionIndex(transitions[curState], element, alphabet, comparer, 0, transitions[curState].Count);
                    if (transitionIndex < 0)
                    {
                        transitionIndex = ~transitionIndex;

                        Int32 alphabetIndex = alphabet.OptimalSearch(element);

                        // add new state
                        nextState = transitions.Count;
                        transitions.Add(new List<SequenceSetTransition>());

                        // add transition from current state to new state                                                
                        var newTransition = new SequenceSetTransition(alphabetIndex, nextState, false);
                        transitions[curState].Insert(transitionIndex, newTransition);
                    }
                    else
                    {
                        nextState = transitions[curState][transitionIndex].StateIndex;
                    }
                }
                var tmp = transitions[curState][transitionIndex];
                transitions[curState][transitionIndex] = new SequenceSetTransition(tmp.AlphabetIndex, tmp.StateIndex, true);
            }

            var result = new SequenceSet<T>();
            result.symbolComparer = comparer;
            result.alphabet = alphabet;
            result.start = 0;
            result.lowerTransitionIndexes = new int[transitions.Count];
            result.transitions = new SequenceSetTransition[transitions.Sum(s => s.Count)];

            for (int i = 0, trIndex = 0; i < transitions.Count; i++)
            {
                result.lowerTransitionIndexes[i] = trIndex;
                for (int j = 0; j < transitions[i].Count; j++, trIndex++)
                {
                    result.transitions[trIndex] = transitions[i][j];
                }
            }

            if (minimize)
            {
                result.Minimize();
            }

            return result;
        }        
        #endregion

        #region FIELDS
        T[] alphabet;
        IList<int> lowerTransitionIndexes;
        IList<SequenceSetTransition> transitions;
        Int32 start;

        /// <summary>
        /// TODO: think of serialization.
        /// </summary>
        IComparer<T> symbolComparer;
        #endregion

        #region PROPERTIES
        #endregion

        #region IEnumerable<IEnumerable<T>>
        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return Enumerate(start).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region METHODS
        public Int32 GetStatesCount()
        {
            return lowerTransitionIndexes.Count;
        }

        public Int32 GetTransitionsCount()
        {
            return transitions.Count;
        }

        public void Minimize()
        {
            RevuzMinimize();
        }

        public bool ContainsSequence(IEnumerable<T> sequence)
        {
            if (!sequence.Any())
            {
                return false;
            }

            Int32 curState = start, trIndex = -1;
            foreach (var element in sequence)
            {
                trIndex = GetTransitionIndex(curState, element);
                if (trIndex >= 0)
                {
                    curState = transitions[trIndex].StateIndex;
                }
                else
                {
                    return false;
                }
            }
            return transitions[trIndex].IsFinal;
        }

        public IEnumerable<IEnumerable<T>> GetByPrefix(IEnumerable<T> prefix)
        {
            if (prefix.Any())
            {
                Int32 curState = start, trIndex = -1;
                foreach (var element in prefix)
                {
                    trIndex = GetTransitionIndex(curState, element);
                    if (trIndex >= 0)
                    {
                        curState = transitions[trIndex].StateIndex;
                    }
                    else
                    {
                        return Enumerable.Empty<IEnumerable<T>>();
                    }
                }
                return Enumerate(transitions[trIndex].StateIndex).Select(sequence => prefix.Concat(sequence));
            }
            else
            {
                return Enumerate(start).Select(sequence => prefix.Concat(sequence));
            }
        }        

        /// <returns>Transition index or bitwise complement to the index where it should be inserted.</returns>
        private static Int32 GetTransitionIndex(IList<SequenceSetTransition> transitions, T input, T[] alphabet, IComparer<T> comparer, Int32 lower, Int32 upper)
        {
            const Int32 BINARY_SEARCH_THRESHOLD = 5;
            if (upper - lower >= BINARY_SEARCH_THRESHOLD)
            {
                // Binary search
                upper--;
                while (lower <= upper)
                {
                    Int32 middle = (lower + upper) / 2;
                    Int32 comparisonResult = comparer.Compare(alphabet[transitions[middle].AlphabetIndex], input);
                    if (comparisonResult == 0)
                        return middle;
                    else if (comparisonResult > 0)
                        upper = middle - 1;
                    else
                        lower = middle + 1;
                }

                return ~lower;
            }
            else
            {
                // Linear search
                for (int i = lower; i < upper; i++)
                {
                    int comp = comparer.Compare(alphabet[transitions[i].AlphabetIndex], input);
                    if (comp == 0)
                    {
                        return i;
                    }
                    else if (comp > 0)
                    {
                        return ~i;
                    }
                }

                return ~upper;
            }
        }               
        
        /// <returns>Transition index or bitwise complement to the index where it should be inserted.</returns>
        protected Int32 GetTransitionIndex(Int32 fromState, T input)
        {            
            Int32 lower = lowerTransitionIndexes[fromState];
            Int32 upper = transitions.GetUpperIndex(lowerTransitionIndexes, fromState);
            return GetTransitionIndex(transitions, input, alphabet, symbolComparer, lower, upper);            
        }        

        protected Int32 GetAlphabetIndex(T item)
        {
            return alphabet.BinarySearch(item, symbolComparer);
        }

        protected IEnumerable<IEnumerable<T>> Enumerate(Int32 fromState)
        {
            Int32 lower = lowerTransitionIndexes[fromState];
            Int32 upper = transitions.GetUpperIndex(lowerTransitionIndexes, fromState);

            for (int i = lower; i < upper; i++)
            {
                var tr = transitions[i];
                T element = alphabet[tr.AlphabetIndex];
                if (tr.IsFinal)
                {
                    yield return new T[] { element };
                }
                foreach (var w in Enumerate(tr.StateIndex).Select(w => w.Prepend(element)))
                {
                    yield return w;
                }
            }
        }

        /// <summary>
        /// Acyclyc DFA minimization algorithm, O(L) time?
        /// </summary>
        protected void RevuzMinimize()
        {
            var registered = new Dictionary<StateSignature, Int32>(new StateSignatureEqualityComparer());
            var mergeList = GetMergeList();
            Register(start, registered, mergeList);

            var registeredArray = registered.ToArray();
            var oldToNewStates = Enumerable.Range(0, registered.Count).ToDictionary(i => registeredArray[i].Value, i => i);
            transitions = new SequenceSetTransition[registered.Sum(r => r.Key.Transitions.Length)];
            lowerTransitionIndexes = new Int32[registered.Count];
            for (int i = 0, transIndex = 0; i < lowerTransitionIndexes.Count; i++)
            {
                // TODO: this could be optimized
                var old = registeredArray[i];
                for (int j = 0; j < old.Key.Transitions.Length; j++)
                {
                    var oldTr = old.Key.Transitions[j];
                    transitions[transIndex + j] = new SequenceSetTransition(oldTr.AlphabetIndex, oldToNewStates[oldTr.StateIndex], oldTr.IsFinal);
                }
                lowerTransitionIndexes[i] = transIndex;
                transIndex += old.Key.Transitions.Length;
            }
            start = oldToNewStates[start];
        }

        Int32 Register(Int32 state, Dictionary<StateSignature, Int32> registered, MergeList mergeList)
        {
            Int32 lower = lowerTransitionIndexes[state];
            Int32 upper = transitions.GetUpperIndex(lowerTransitionIndexes, state);
            Int32 registeredState;

            for (int i = lower; i < upper; i++)
            {
                Int32 sj = transitions[i].StateIndex;
                registeredState = Register(sj, registered, mergeList);
                if (registeredState >= 0)
                {
                    // state sj has to be substituted with registeredState                    
                    foreach (var t in mergeList.GetTransitionIndexes(sj))
                    {
                        var transition = transitions[t];
                        transitions[t] = new SequenceSetTransition(transition.AlphabetIndex, registeredState, transition.IsFinal);
                    }
                    mergeList.Merge(registeredState, sj);
                }
            }

            var sigTransitions = new SequenceSetTransition[upper - lower];
            for (int i = lower; i < upper; i++)
            {
                sigTransitions[i - lower] = transitions[i];
            }
            var signature = new StateSignature(sigTransitions);
            if (registered.TryGetValue(signature, out registeredState))
            {
                return registeredState;
            }
            else
            {
                registered.Add(signature, state);
            }
            return -1;
        }

        MergeList GetMergeList()
        {
            var merge = new MergeList(lowerTransitionIndexes.Count, transitions.Count);
            for (int i = 0; i < transitions.Count; i++)
            {
                merge.Add(i, transitions[i].StateIndex);
            }
            return merge;
        }
        #endregion
    }
}
