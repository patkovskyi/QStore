namespace QStore.Tests.QStringSetTests
{
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Contains
    {
        public static void ContainsTestHelper(IComparer<char> comparer, string[] dictionary, string[] notInDictionary)
        {
            var target = QStringSet.Create(dictionary, comparer);

            foreach (var word in dictionary)
            {
                Assert.IsTrue(target.Contains(word));
            }

            foreach (var wrongWord in notInDictionary)
            {
                Assert.IsFalse(target.Contains(wrongWord));
            }
        }

        public static void ContainsTestHelper(string[] dictionary, string[] notInDictionary)
        {
            ContainsTestHelper(Comparer<char>.Default, dictionary, notInDictionary);
        }

        [TestMethod]
        [DeploymentItem(TestData.BaseformsDeployedPath)]
        public void ContainsBaseforms()
        {
            ContainsTestHelper(
                dictionary: File.ReadAllLines(TestData.BaseformsDeployedPath, TestData.Encoding),
                notInDictionary: new string[0]);
        }

        [TestMethod]
        public void ContainsEmptySequence()
        {
            ContainsTestHelper(dictionary: new[] { "aa", string.Empty, "ab" }, notInDictionary: new[] { "a", " " });
        }

        [TestMethod]
        public void ContainsSimple1()
        {
            ContainsTestHelper(
                dictionary: new[] { "aa", "ab", "ac", "abc" },
                notInDictionary: new[] { "aaa", "ab ", " ab", "aab", "aca", "abcabc", string.Empty });
        }
    }
}