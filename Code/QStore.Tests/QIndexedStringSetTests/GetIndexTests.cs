namespace QStore.Tests.QIndexedStringSetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Strings;
    using QStore.Tests.Comparers;
    using QStore.Tests.Helpers;

    [TestClass]
    public class GetIndexTests
    {
        public static void GetIndexTestHelper(IComparer<char> comparer, params string[] strings)
        {
            var target = QIndexedStringSet.Create(strings, comparer);
            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected = strings.OrderBy(s => s, sequenceComparer).ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(i, target.GetIndex(expected[i]));
            }
        }

        public static void GetIndexTestHelper(params string[] strings)
        {
            GetIndexTestHelper(Comparer<char>.Default, strings);
        }

        [TestMethod]
        public void GetIndexSimple1()
        {
            GetIndexTestHelper("aa", "ab", "ac", "abc");
        }

        [TestMethod]
        public void GetIndexSimple2()
        {
            GetIndexTestHelper("one", "two", "three", "four", "five");
        }

        [TestMethod]
        public void GetIndexNullSequence()
        {
            const string ParamName = "sequence";
            var target = QIndexedStringSet.Create(new[] { "aa", "ab" }, Comparer<char>.Default);
            var e = ExceptionAssert.Throws<ArgumentNullException>(() => target.GetIndex(null));
            Assert.AreEqual(new ArgumentNullException(ParamName).Message, e.Message);
        }

        [TestMethod]
        public void GetIndexEmptySequence()
        {
            var target = QIndexedStringSet.Create(new[] { "aa", "ab" }, Comparer<char>.Default);
            var e = ExceptionAssert.Throws<ArgumentException>(() => target.GetIndex(string.Empty));
            Assert.AreEqual(ErrorMessages.EmptySequencesAreNotSupported, e.Message);
        }
    }
}
