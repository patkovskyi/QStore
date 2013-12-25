namespace QStore.Tests.QStringMapTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.TestComparers;

    [TestClass]
    public class EnumerateByPrefix
    {
        public static void EnumerateByPrefixTestHelper(
            IComparer<char> comparer, string prefix, params string[] words)
        {
            var map = QStringMap<int>.Create(words, comparer);
            foreach (var word in words)
            {
                map[word] = word.GetHashCode();
            }

            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected =
                words.Where(s => s.StartsWith(prefix, StringComparison.Ordinal))
                     .OrderBy(s => s, sequenceComparer)
                     .Select(s => new KeyValuePair<string, int>(s, s.GetHashCode()))
                     .ToArray();

            var actual = map.EnumerateByPrefixWithValue(prefix).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        public static void EnumerateByPrefixTestHelper(string prefix, params string[] words)
        {
            EnumerateByPrefixTestHelper(Comparer<char>.Default, prefix, words);
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