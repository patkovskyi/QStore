namespace QStore.Tests.QStringSetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.TestComparers;

    [TestClass]
    public class EnumerateByPrefix
    {
        public static void EnumerateByPrefixTestHelper(IComparer<char> comparer, string prefix, params string[] strings)
        {
            var target = QStringSet.Create(strings, comparer);
            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected =
                strings.Where(s => s.StartsWith(prefix, StringComparison.Ordinal))
                       .OrderBy(s => s, sequenceComparer)
                       .ToArray();

            var actual = target.EnumerateByPrefix(prefix).ToArray();
            CollectionAssert.AreEqual(expected, actual, sequenceComparer);
        }

        public static void EnumerateByPrefixTestHelper(string prefix, params string[] strings)
        {
            EnumerateByPrefixTestHelper(Comparer<char>.Default, prefix, strings);
        }

        [TestMethod]
        public void EnumerateByPrefixEmptySequence()
        {
            EnumerateByPrefixTestHelper(string.Empty, "aa", "bb", "abc", string.Empty);
        }

        [TestMethod]
        public void EnumerateByPrefixSimple1()
        {
            EnumerateByPrefixTestHelper("a", "aa", "ab", "ac", "abc");
        }

        [TestMethod]
        public void EnumerateByPrefixSimple2()
        {
            EnumerateByPrefixTestHelper("a", "aa", "bb", "abc", string.Empty);
        }

        [TestMethod]
        public void EnumerateByPrefixSimple3()
        {
            EnumerateByPrefixTestHelper("ab", "aa", "ab", "ac", "abc");
        }
    }
}