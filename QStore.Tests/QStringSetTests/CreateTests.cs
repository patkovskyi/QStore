namespace QStore.Tests.QStringSetTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.Comparers;

    [TestClass]
    public class CreateTests
    {
        public static void CreateTestHelper(IComparer<char> comparer, params string[] strings)
        {
            var target = QStringSet.Create(strings, comparer);
            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected = strings.OrderBy(s => s, sequenceComparer).ToArray();
            var actual = target.ToArray();
            CollectionAssert.AreEqual(expected, actual, sequenceComparer);
        }

        [TestMethod]
        public void CreateTestSimple()
        {
            CreateTestHelper(Comparer<char>.Default, "one", "two", "three", "four", "five");
        }

        [TestMethod]
        public void CreateTestZaliznyak()
        {
            CreateTestHelper(Comparer<char>.Default, TestData.Zaliznyak);
        }

        [TestMethod]
        public void CreateTestZaliznyakBaseforms()
        {
            CreateTestHelper(Comparer<char>.Default, TestData.ZaliznyakBaseforms);
        }
    }
}