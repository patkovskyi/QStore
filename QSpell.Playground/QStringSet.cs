// -----------------------------------------------------------------------
// <copyright file="QStringSet.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace QSpell.Playground
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QStringSet : QSet<char>, IEnumerable<string>
    {
        public static QStringSet Create(IEnumerable<string> strings, Comparer<char> comparer)
        {
            throw new NotImplementedException();
        }

        public new IEnumerator<string> GetEnumerator()
        {
            return this.Enumerate().Select(s => new string(s.ToArray())).GetEnumerator();
        }
    }
}
