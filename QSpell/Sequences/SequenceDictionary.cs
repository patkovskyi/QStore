using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Extensions;
using QSpell.Comparers;

namespace QSpell.Sequences
{
    public class SequenceDictionary<T, V> : SequenceSet<T>, IEnumerable<KeyValuePair<IEnumerable<T>, V>>
    {
        #region CONSTRUCTORS
        public static SequenceDictionary<T, V> Create(IEnumerable<KeyValuePair<IEnumerable<T>, V>> sequences, IComparer<T> comparer, bool minimize)
        {
            var result = SequenceSet<T>.Create<SequenceDictionary<T, V>>(sequences.Select(s => s.Key), comparer, minimize);
            result.values = sequences.OrderBy(s => s.Key, new SequenceComparer<T>(comparer)).Select(s => s.Value).ToArray();
            if (!minimize)
            {
                result.RefreshPaths();
            }
            return result;
        }
        #endregion

        #region FIELDS
        protected Int32[] pathsLeft;
        protected V[] values;
        #endregion

        #region PROPERTIES
        #endregion

        #region METHODS
        public override void Minimize()
        {
            base.Minimize();
            RefreshPaths();
        }

        public bool TryGetValue(IEnumerable<T> sequence, out V value)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            List<Int32> transitionPath = null;
            if (ContainsSequence(sequence, out transitionPath))
            {
                Int32 valueIndex = 0;
                for (int i = 0; i < transitionPath.Count; i++)
                {
                    valueIndex += pathsLeft[transitionPath[i]];
                }
                value = values[valueIndex];
                return true;
            }
            else
            {
                value = default(V);
                return false;
            }
        }

        public V this[IEnumerable<T> sequence]
        {
            get
            {                
                V value;
                if (TryGetValue(sequence, out value))
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }

        protected void RefreshPaths()
        {
            pathsLeft = new Int32[transitions.Count];
            RefreshPathsFrom(start, new Dictionary<Int32, Int32>());
        }

        protected Int32 RefreshPathsFrom(Int32 state, Dictionary<Int32, Int32> memoizeCache)
        {
            Int32 pathsLeftCounter = 0;
            if (memoizeCache.TryGetValue(state, out pathsLeftCounter))
            {
                return pathsLeftCounter;
            }
            else
            {
                pathsLeftCounter = 0;
            }

            Int32 lower = lowerTransitionIndexes[state];
            Int32 upper = transitions.GetUpperIndex(lowerTransitionIndexes, state);

            if (upper > lower)
            {
                for (int i = lower; i < upper; i++)
                {
                    pathsLeft[i] = pathsLeftCounter;
                    pathsLeftCounter += RefreshPathsFrom(transitions[i].StateIndex, memoizeCache);
                }
            }
            else
            {
                pathsLeftCounter = 1;
            }

            memoizeCache.Add(state, pathsLeftCounter);
            return pathsLeftCounter;
        }
        #endregion

        #region IEnumerator<KeyValuePair<IEnumerable<T>, V>>
        public new IEnumerator<KeyValuePair<IEnumerable<T>, V>> GetEnumerator()
        {
            return Enumerate(start).Select((s, i) => new KeyValuePair<IEnumerable<T>, V>(s, values[i])).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
