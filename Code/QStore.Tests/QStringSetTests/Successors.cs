namespace QStore.Tests.QStringSetTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [Ignore]
    public class Successors
    {
        [TestMethod]
        public void AfterLastWord()
        {
            this.SuccessorsTestHelper(new[] { "bc", "bb", "cd" }, "cde", new string[0]);
        }

        [TestMethod]
        public void AfterLastWord2()
        {
            this.SuccessorsTestHelper(new[] { "bc", "bb", "cd" }, "dd", new string[0]);
        }

        [TestMethod]
        public void BeforeFirstWord()
        {
            this.SuccessorsTestHelper(new[] { "bc", "bb", "cd" }, "a", new[] { "bb", "bc", "cd" });
        }

        [TestMethod]
        public void BeforeFirstWord2()
        {
            this.SuccessorsTestHelper(new[] { "bc", "ab", "bb", "cd" }, "a", new[] { "ab", "bb", "bc", "cd" });
        }

        [TestMethod]
        public void FirstWord()
        {
            this.SuccessorsTestHelper(new[] { "bc", "a", "cd", "ab" }, "a", new[] { "ab", "bc", "cd" });
        }

        [TestMethod]
        public void LastWord()
        {
            this.SuccessorsTestHelper(new[] { "bc", "bb", "cd" }, "cd", new string[0]);
        }

        [TestMethod]
        public void StringEmpty()
        {
            this.SuccessorsTestHelper(new[] { string.Empty, "ab", "bc" }, string.Empty, new[] { "ab", "bc" });
        }

        public void SuccessorsTestHelper(string[] words, string testWord, string[] expectedSuccessors)
        {
            var qset = QStringSet.Create(words, Comparer<char>.Default);
            var successors = qset.Successors(testWord).ToArray();
            CollectionAssert.AreEqual(expectedSuccessors, successors);
        }
    }
}