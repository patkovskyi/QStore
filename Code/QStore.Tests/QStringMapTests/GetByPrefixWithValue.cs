namespace QStore.Tests.QStringMapTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.TestComparers;

    [TestClass]
    public class GetByPrefixWithValue
    {
        public static void GetByPrefixWithValueTestHelper(
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

        public static void GetByPrefixWithValueTestHelper(string prefix, params string[] words)
        {
            GetByPrefixWithValueTestHelper(Comparer<char>.Default, prefix, words);
        }

        [TestMethod]
        public void GetByPrefixWithValueEmptySequence()
        {
            GetByPrefixWithValueTestHelper(string.Empty, "aa", "bb", "abc", string.Empty);
        }

        [TestMethod]
        public void GetByPrefixWithValueSimple1()
        {
            GetByPrefixWithValueTestHelper("a", "aa", "ab", "ac", "abc");
        }

        [TestMethod]
        public void GetByPrefixWithValueSimple2()
        {
            GetByPrefixWithValueTestHelper("a", "aa", "bb", "abc", string.Empty);
        }

        [TestMethod]
        public void GetByPrefixWithValueSimple3()
        {
            GetByPrefixWithValueTestHelper("ab", "aa", "ab", "ac", "abc");
        }
    }
}