namespace QStore.Tests.QStringMapTests
{
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Strings;

    [TestClass]
    public class IndexerTests
    {
        public static void IndexerTestHelper(IComparer<char> comparer, params string[] words)
        {
            var map = QStringMap<int>.Create(words, Comparer<char>.Default);
            foreach (var word in words)
            {
                map[word] = word.GetHashCode();
            }

            foreach (var word in words)
            {
                Assert.AreEqual(word.GetHashCode(), map[word]);
            }
        }

        public static void IndexerTestHelper(params string[] strings)
        {
            IndexerTestHelper(Comparer<char>.Default, strings);
        }

        [TestMethod]
        [DeploymentItem(TestData.BaseformsDeployedPath)]
        public void IndexerBaseforms()
        {
            IndexerTestHelper(File.ReadAllLines(TestData.BaseformsDeployedPath, TestData.Encoding));
        }

        [TestMethod]
        public void IndexerEmptySequence()
        {
            IndexerTestHelper("a", "ab", string.Empty);
        }
    }
}