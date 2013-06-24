namespace QStore.Tests.QStringSetTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

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
            Console.WriteLine("QStringSet.Create() took {0}", watch.Elapsed);
            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected = strings.OrderBy(s => s, sequenceComparer).ToArray();
            watch.Restart();
            var actual = target.ToArray();
            Console.WriteLine("QStringSet.ToArray() took {0}", watch.Elapsed);
            CollectionAssert.AreEqual(expected, actual, sequenceComparer);
        }

        public static void DefaultTestHelper(params string[] strings)
        {
            CreateTestHelper(Comparer<char>.Default, strings);
        }

        [TestMethod]
        public void CreateTestSimple1()
        {
            DefaultTestHelper("one", "two", "three", "four", "five");
        }

        [TestMethod]
        public void CreateTestSimple2()
        {
            DefaultTestHelper("aa", "ab", "ac", "abc");
        }

        [TestMethod]
        [DeploymentItem(@"..\TestData\Zaliznyak-1251.txt")]
        public void CreateTestZaliznyak()
        {
            DefaultTestHelper(File.ReadAllLines(@"Zaliznyak-1251.txt", Encoding.GetEncoding(1251)));
        }

        [TestMethod]
        [DeploymentItem(@"..\TestData\Zaliznyak-baseforms-1251.txt")]
        public void CreateTestZaliznyakBaseforms()
        {
            DefaultTestHelper(File.ReadAllLines(@"Zaliznyak-baseforms-1251.txt", Encoding.GetEncoding(1251)));
        }

        [TestMethod]
        public void CreateTestEmptySequenceException()
        {
            var e =
                ExceptionAssert.Throws<ArgumentException>(
                    () => QStringSet.Create(new[] { "a", string.Empty, "ab" }, Comparer<char>.Default));
            Assert.AreEqual(e.Message, "Empty sequences are not allowed!");
        }

        [TestMethod]
        public void CreateTestDuplicateKeyException()
        {
            var e =
                ExceptionAssert.Throws<ArgumentException>(
                    () => QStringSet.Create(new[] { "ab", "a", "ab" }, Comparer<char>.Default));
            Assert.AreEqual(e.Message, "An element with Key = \"ab\" already exists.");
        }
    }
}