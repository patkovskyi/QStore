namespace QStore.Extensions
{
    using System;
    using System.Collections.Generic;

    internal static class IEnumerableExtensions
    {
        #region Methods

        /// <summary>Adds a single element to the end of an IEnumerable.</summary>
        /// <typeparam name="T">Type of enumerable to return.</typeparam>
        /// <returns>IEnumerable containing all the input elements, followed by the
        /// specified additional element.</returns>
        internal static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
        {
            if (source == null) throw new ArgumentNullException("source");
            return ConcatIterator(element, source, false);
        }

        /// <summary>
        /// Adds a single element to the desired position.
        /// Will add at start if you pass zero or negative position.
        /// Will add at the end if you pass more than source.LongCount() position.
        /// </summary>        
        internal static IEnumerable<T> Insert<T>(this IEnumerable<T> source, T element, long position)
        {
            var e = source.GetEnumerator();
            bool canMove = e.MoveNext();
            for (int i = 0; i < position && canMove; i++)
            {
                yield return e.Current;
                canMove = e.MoveNext();
            }

            yield return element;
            while (canMove)
            {
                yield return e.Current;
                canMove = e.MoveNext();
            }
        }

        /// <summary>Adds a single element to the start of an IEnumerable.</summary>
        /// <typeparam name="T">Type of enumerable to return.</typeparam>
        /// <returns>IEnumerable containing the specified additional element, followed by
        /// all the input elements.</returns>
        internal static IEnumerable<T> Prepend<T>(this IEnumerable<T> tail, T head)
        {
            if (tail == null) throw new ArgumentNullException("tail");
            return ConcatIterator(head, tail, true);
        }

        private static IEnumerable<T> ConcatIterator<T>(T extraElement, IEnumerable<T> source, bool insertAtStart)
        {
            if (insertAtStart) yield return extraElement;
            foreach (var e in source)
            {
                yield return e;
            }

            if (!insertAtStart) yield return extraElement;
        }

        #endregion
    }
}