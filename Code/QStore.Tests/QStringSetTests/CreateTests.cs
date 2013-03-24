namespace QStore.Tests.QStringSetTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.Comparers;

    [TestClass]
    public class CreateTests
    {
        public static void CreateTestHelper(
            IComparer<char> comparer, IEqualityComparer<char> equalityComparer, params string[] strings)
        {
            var target = QStringSet.Create(strings, comparer, equalityComparer);
            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected = strings.OrderBy(s => s, sequenceComparer).ToArray();
            var actual = target.ToArray();
            CollectionAssert.AreEqual(expected, actual, sequenceComparer);
        }

        public static void DefaultTestHelper(params string[] strings)
        {
            CreateTestHelper(Comparer<char>.Default, EqualityComparer<char>.Default, strings);
        }

        [TestMethod]
        public void CreateTestSimple()
        {
            DefaultTestHelper("one", "two", "three", "four", "five");
        }

        [TestMethod]
        public void CreateTestZaliznyak()
        {
            DefaultTestHelper(TestData.Zaliznyak);
        }

        [TestMethod]
        public void CreateTestZaliznyakBaseforms()
        {
            DefaultTestHelper(TestData.ZaliznyakBaseforms);
        }
    }
}