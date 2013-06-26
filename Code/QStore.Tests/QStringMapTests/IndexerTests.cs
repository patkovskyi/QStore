namespace QStore.Tests.QStringMapTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Strings;

    [TestClass]
    public class IndexerTests
    {
        [TestMethod]
        [DeploymentItem(TestData.BaseformsDeployedPath)]
        public void BaseformsIndexerTest()
        {
            var words = File.ReadAllLines(TestData.BaseformsDeployedPath, TestData.Encoding);
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
    }
}
