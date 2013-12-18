namespace QStore.Tests.QStringMapTests
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.TestComparers;
    using QStore.Tests.TestHelpers;

    [TestClass]
    public class Serialization
    {
        public static void SerializationTestHelper(
            Func<QStringMap<int>, QStringMap<int>> serializationLoop, params string[] words)
        {
            var comparer = new NonSerializableComparer<char>();
            var map = QStringMap<int>.Create(words, comparer);
            foreach (var word in words)
            {
                map[word] = word.GetHashCode();
            }

            map = serializationLoop(map);
            map.SetComparer(comparer);
            Assert.AreEqual(words.Length, map.Count);

            foreach (var word in words)
            {
                Assert.AreEqual(word.GetHashCode(), map[word]);
            }
        }        

        [TestMethod]
        [DeploymentItem(TestData.BaseformsDeployedPath)]
        public void BinaryBaseforms()
        {
            SerializationTestHelper(
                SerializationHelper.BinaryLoop, File.ReadAllLines(TestData.BaseformsDeployedPath, TestData.Encoding));
        }

        [TestMethod]
        [DeploymentItem(TestData.BaseformsDeployedPath)]
        public void BsonBaseforms()
        {
            SerializationTestHelper(
                SerializationHelper.BsonLoop, File.ReadAllLines(TestData.BaseformsDeployedPath, TestData.Encoding));
        }

        [TestMethod]
        [DeploymentItem(TestData.BaseformsDeployedPath)]
        public void DataContractBaseforms()
        {
            SerializationTestHelper(
                SerializationHelper.DataContractLoop,
                File.ReadAllLines(TestData.BaseformsDeployedPath, TestData.Encoding));
        }

        [TestMethod]
        [DeploymentItem(TestData.BaseformsDeployedPath)]
        public void ProtoBaseforms()
        {
            SerializationTestHelper(
                SerializationHelper.ProtoLoop, File.ReadAllLines(TestData.BaseformsDeployedPath, TestData.Encoding));
        }
    }
}