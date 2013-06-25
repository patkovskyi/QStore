namespace QStore.Tests.QStringSetTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.Comparers;
    using QStore.Tests.Helpers;

    [TestClass]
    public class CreateTests
    {
        public static void CreateTestHelper(IComparer<char> comparer, params string[] strings)
        {
            var watch = new Stopwatch();
            watch.Start();
            var target = QStringSet.Create(strings, comparer);
            Console.WriteLine(@"QStringSet.Create() took {0}", watch.Elapsed);
            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected = strings.OrderBy(s => s, sequenceComparer).ToArray();
            watch.Restart();
            var actual = target.ToArray();
            Console.WriteLine(@"QStringSet.ToArray() took {0}", watch.Elapsed);
            CollectionAssert.AreEqual(expected, actual, sequenceComparer);
        }

        public static void CreateTestHelper(params string[] strings)
        {
            CreateTestHelper(Comparer<char>.Default, strings);
        }

        [TestMethod]
        [DeploymentItem(TestData.BaseformsDeployedPath)]
        public void CreateBaseforms()
        {
            CreateTestHelper(File.ReadAllLines(TestData.BaseformsDeployedPath, TestData.Encoding));
        }

        [TestMethod]
        public void CreateSimple1()
        {
            CreateTestHelper("aa", "ab", "ac", "abc");
        }

        [TestMethod]
        public void CreateSimple2()
        {
            CreateTestHelper("one", "two", "three", "four", "five");
        }

        [TestMethod]
        public void CreateTestDuplicateKeyException()
        {
            var e =
                ExceptionAssert.Throws<ArgumentException>(
                    () => QStringSet.Create(new[] { "ab", "a", "ab" }, Comparer<char>.Default));
            Assert.AreEqual(string.Format(ErrorMessages.DuplicateKey, "ab"), e.Message);
        }

        [TestMethod]
        public void CreateTestEmptySequenceException()
        {
            var e =
                ExceptionAssert.Throws<ArgumentException>(
                    () => QStringSet.Create(new[] { "a", string.Empty, "ab" }, Comparer<char>.Default));
            Assert.AreEqual(ErrorMessages.EmptySequencesAreNotSupported, e.Message);
        }

        [TestMethod]
        [DeploymentItem(TestData.ZaliznyakSolutionPath)]
        public void CreateZaliznyak()
        {
            CreateTestHelper(File.ReadAllLines(TestData.ZaliznyakDeployedPath, TestData.Encoding));
        }
    }
}