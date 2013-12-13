namespace QStore.Tests.SerializationTests
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Structs;
    using QStore.Tests.Comparers;
    using QStore.Tests.Helpers;

    [TestClass]
    public class QStringMapSerializationTests
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
        public void HowMuchIsTheFish()
        {
            int size = Marshal.SizeOf(new QTransition());
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