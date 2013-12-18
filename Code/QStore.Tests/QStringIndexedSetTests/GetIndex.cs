namespace QStore.Tests.QStringIndexedSetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.TestComparers;
    using QStore.Tests.TestHelpers;

    [TestClass]
    public class GetIndex
    {
        public static void GetIndexTestHelper(IComparer<char> comparer, params string[] strings)
        {
            var target = QStringIndexedSet.Create(strings, comparer);
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
        public void GetIndexEmptySequence()
        {
            GetIndexTestHelper("aa", "ab", string.Empty);
        }

        [TestMethod]
        public void GetIndexNegative()
        {
            var target = QStringIndexedSet.Create(new[] { "ba", "bc" }, Comparer<char>.Default);
            Assert.AreEqual(~0, target.GetIndex(string.Empty));
            Assert.AreEqual(~0, target.GetIndex("aa"));
            Assert.AreEqual(~1, target.GetIndex("baa"));
            Assert.AreEqual(~1, target.GetIndex("bb"));
            Assert.AreEqual(~2, target.GetIndex("bca"));
            Assert.AreEqual(~2, target.GetIndex("bd"));
        }

        [TestMethod]
        public void GetIndexNullSequence()
        {
            const string ParamName = "sequence";
            var target = QStringIndexedSet.Create(new[] { "aa", "ab" }, Comparer<char>.Default);
            var e = ExceptionAssert.Throws<ArgumentNullException>(() => target.GetIndex(null));
            Assert.AreEqual(new ArgumentNullException(ParamName).Message, e.Message);
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
    }
}