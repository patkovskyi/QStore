namespace QStore.Tests.QStringSetTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [Ignore]
    public class Predecessors
    {
        [TestMethod]
        public void AfterLastWord()
        {
            this.PredecessorsTestHelper(new[] { "bc", "bb", "cd" }, "dd", new[] { "bb", "bc", "cd" });
        }

        [TestMethod]
        public void AfterLastWord2()
        {
            this.PredecessorsTestHelper(new[] { "bc", "bb", "cd" }, "dd", new[] { "bb", "bc", "cd" });
        }

        [TestMethod]
        public void BeforeFirstWord()
        {
            this.PredecessorsTestHelper(new[] { "bc", "bb", "cd" }, "a", new string[0]);
        }

        [TestMethod]
        public void BeforeFirstWord2()
        {
            this.PredecessorsTestHelper(new[] { "bc", "bb", "cd", "ab" }, "a", new string[0]);
        }

        [TestMethod]
        public void FirstWord()
        {
            this.PredecessorsTestHelper(new[] { "bc", "a", "cd", "ab" }, "a", new string[0]);
        }

        [TestMethod]
        public void LastWord()
        {
            this.PredecessorsTestHelper(new[] { "bc", "bb", "cd" }, "cd", new[] { "bb", "bc" });
        }

        public void PredecessorsTestHelper(string[] words, string testWord, string[] expectedPredecessors)
        {
            var qset = QStringSet.Create(words, Comparer<char>.Default);
            var predecessors = qset.Predecessors(testWord).ToArray();
            CollectionAssert.AreEqual(expectedPredecessors, predecessors);
        }

        [TestMethod]
        public void StringEmpty()
        {
            this.PredecessorsTestHelper(new[] { string.Empty, "ab", "bc" }, string.Empty, new string[0]);
        }
    }
}