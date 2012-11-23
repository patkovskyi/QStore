namespace QStore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QSpell.Playground;

    [TestClass]
    public class QStringSetTests
    {
        [TestMethod]
        public void CreateFromSingleString()
        {
            var comparer = Comparer<char>.Default;
            IEnumerable<string> sequences = new[] { "string" };
            QStringSet target = QStringSet.Create(sequences, comparer);

            Assert.Inconclusive();
            //var expected = sequences.OrderBy(s => s, comparer).ToArray();
            //var actual = target.ToArray<string>();
            //CollectionAssert.AreEqual(expected, actual, comparer);
        }

        [TestMethod]
        public void CreateFromMillionRandomStrings()
        {
            var comparer = Comparer<char>.Default;
            IEnumerable<string> sequences = Enumerable.Repeat(0, 1000000).Select(s => Path.GetRandomFileName()).ToArray();
            QStringSet target = QStringSet.Create(sequences, comparer);

            Assert.Inconclusive();
            //var expected = sequences.OrderBy(s => s, comparer).ToArray();
            //var actual = target.ToArray<string>();
            //CollectionAssert.AreEqual(expected, actual, comparer);
        }
    }
}
