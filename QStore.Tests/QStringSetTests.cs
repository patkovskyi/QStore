namespace QStore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QStringSetTests
    {
        [TestMethod]
        public void CreateFromSingleString()
        {
            StringComparer comparer = StringComparer.Ordinal;
            IEnumerable<string> sequences = new[] { "string" };
            QStringSet target = QStringSet.Create(sequences, comparer);
            
            var expected = sequences.OrderBy(s => s, comparer).ToArray();
            var actual = target.ToArray<string>();
            CollectionAssert.AreEqual(expected, actual, comparer);
        }

        [TestMethod]
        public void CreateFromMillionRandomStrings()
        {
            StringComparer comparer = StringComparer.Ordinal;
            IEnumerable<string> sequences = Enumerable.Repeat(0, 1000000).Select(s => Path.GetRandomFileName()).ToArray();
            QStringSet target = QStringSet.Create(sequences, comparer);

            var expected = sequences.OrderBy(s => s, comparer).ToArray();
            var actual = target.ToArray<string>();
            CollectionAssert.AreEqual(expected, actual, comparer);
        }
    }
}
