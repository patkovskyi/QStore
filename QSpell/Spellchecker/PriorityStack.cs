using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSpell.Spellchecker
{
    public class PriorityStack<P, V>
    {
        private SortedDictionary<P, Stack<V>> stack;

        public PriorityStack(IComparer<P> comparer)
        {
            stack = new SortedDictionary<P, Stack<V>>(comparer);
        }
        public PriorityStack() : this(Comparer<P>.Default) { }

        public void Push(P priority, V value)
        {
            Stack<V> q;
            if (!stack.TryGetValue(priority, out q))
            {
                q = new Stack<V>(50);
                stack.Add(priority, q);
            }            
            q.Push(value);
        }

        public V Pop()
        {            
            var pair = stack.FirstOrDefault();
            if (pair.Value == null)
            {
                throw new InvalidOperationException("Stack is empty");
            }
            var v = pair.Value.Pop();
            if (pair.Value.Count == 0) // nothing left of the top priority.
                stack.Remove(pair.Key);
            return v;
        }

        public V Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Stack is empty");
            }
            return stack.First().Value.Peek();            
        }

        public P PeekPriority()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Stack is empty");
            }
            return stack.First().Key;
        }

        public bool IsEmpty
        {
            get { return stack.Count == 0; }
        }
    }
}
