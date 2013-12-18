namespace QStore.Tests.QStringSetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.TestComparers;

    [TestClass]
    public class GetByPrefix
    {
        public static void GetByPrefixTestHelper(IComparer<char> comparer, string prefix, params string[] strings)
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

        public static void GetByPrefixTestHelper(string prefix, params string[] strings)
        {
            GetByPrefixTestHelper(Comparer<char>.Default, prefix, strings);
        }

        [TestMethod]
        public void GetByPrefixEmptySequence()
        {
            GetByPrefixTestHelper(string.Empty, "aa", "bb", "abc", string.Empty);
        }

        [TestMethod]
        public void GetByPrefixSimple1()
        {
            GetByPrefixTestHelper("a", "aa", "ab", "ac", "abc");
        }

        [TestMethod]
        public void GetByPrefixSimple2()
        {
            GetByPrefixTestHelper("a", "aa", "bb", "abc", string.Empty);
        }

        [TestMethod]
        public void GetByPrefixSimple3()
        {
            GetByPrefixTestHelper("ab", "aa", "ab", "ac", "abc");
        }
    }
}