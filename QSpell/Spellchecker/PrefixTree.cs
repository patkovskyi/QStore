using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Extensions;

namespace QSpell.Spellchecker
{
    internal class PrefixTree<T>
    {
        protected Dictionary<char, PrefixTree<T>> _childTrees = new Dictionary<char, PrefixTree<T>>();
        protected List<T> _items = new List<T>();

        public IComparer<T> Comparer { get; protected set; }

        public PrefixTree(IComparer<T> comparer)
        {
            Comparer = comparer;
        }

        public void Add(string prefix, T item)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            if (item == null)
            {
                throw new ArgumentNullException("items");
            }

            if (prefix == string.Empty)
            {
                _items.SortedInsert(item, Comparer);
            }
            else
            {
                PrefixTree<T> child = null;
                if (!_childTrees.TryGetValue(prefix[0], out child))
                {
                    child = new PrefixTree<T>(Comparer);
                    _childTrees.Add(prefix[0], child);
                }
                child.Add(prefix.Substring(1), item);
            }
        }

        public IEnumerable<T> Get(string prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            PrefixTree<T> child = null;
            if (prefix == string.Empty || !_childTrees.TryGetValue(prefix[0], out child))
            {
                return _items;
            }
            else
            {
                return SortedSequence<T>.Join(Comparer, _items, child.Get(prefix.Substring(1)));
            }
        }
    }
}
