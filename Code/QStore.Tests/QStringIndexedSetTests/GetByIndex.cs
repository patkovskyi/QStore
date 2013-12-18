namespace QStore.Tests.QStringIndexedSetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.TestComparers;
    using QStore.Tests.TestHelpers;

    [TestClass]
    public class GetByIndex
    {
        public static void GetByIndexTestHelper(IComparer<char> comparer, params string[] strings)
        {
            var target = QStringIndexedSet.Create(strings, comparer);
            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected = strings.OrderBy(s => s, sequenceComparer).ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], target.GetByIndex(i));
            }
        }

        public static void GetByIndexTestHelper(params string[] strings)
        {
            GetByIndexTestHelper(Comparer<char>.Default, strings);
        }

        [TestMethod]
        public void GetByIndexEmptySequence()
        {
            GetByIndexTestHelper("one", "two", string.Empty);
        }

        [TestMethod]
        public void GetByIndexOutOfRangeException1()
        {
            const int BadIndex = -1;
            var target = QStringIndexedSet.Create(new[] { "aa", "ab" }, Comparer<char>.Default);
            var e = ExceptionAssert.Throws<IndexOutOfRangeException>(() => target.GetByIndex(BadIndex));
            Assert.AreEqual(string.Format(ErrorMessages.IndexOutOfRange, BadIndex, target.Count), e.Message);
        }

        [TestMethod]
        public void GetByIndexOutOfRangeException2()
        {
            const int BadIndex = 3;
            var target = QStringIndexedSet.Create(new[] { "aa", "ab" }, Comparer<char>.Default);
            var e = ExceptionAssert.Throws<IndexOutOfRangeException>(() => target.GetByIndex(BadIndex));
            Assert.AreEqual(string.Format(ErrorMessages.IndexOutOfRange, BadIndex, target.Count), e.Message);
        }

        [TestMethod]
        public void GetByIndexSimple1()
        {
            GetByIndexTestHelper("aa", "ab", "ac", "abc");
        }

        [TestMethod]
        public void GetByIndexSimple2()
        {
            GetByIndexTestHelper("one", "two", "three", "four", "five");
        }
    }
}