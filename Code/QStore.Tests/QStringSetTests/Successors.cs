namespace QStore.Tests.QStringSetTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Successors
    {
        public void SuccessorsTestHelper(string[] words, string testWord, string[] expectedSuccessors)
        {            
            var qset = QStringSet.Create(words, Comparer<char>.Default);
            var successors = qset.Successors(testWord).ToArray();            
            CollectionAssert.AreEqual(expectedSuccessors, successors);
        }

        [TestMethod]
        public void StringEmpty()
        {
            SuccessorsTestHelper(new[] { string.Empty, "ab", "bc" }, string.Empty, new[] { "ab", "bc" });
        }

        [TestMethod]
        public void BeforeFirstWord()
        {
            SuccessorsTestHelper(new[] { "bc", "bb", "cd" }, "a", new[] { "bb", "bc", "cd" });
        }

        [TestMethod]
        public void BeforeFirstWord2()
        {
            SuccessorsTestHelper(new[] { "bc", "ab", "bb", "cd" }, "a", new[] { "ab", "bb", "bc", "cd" });
        }

        [TestMethod]
        public void FirstWord()
        {
            SuccessorsTestHelper(new[] { "bc", "a", "cd", "ab" }, "a", new[] { "ab", "bc", "cd" });
        }                

        [TestMethod]
        public void LastWord()
        {
            SuccessorsTestHelper(new[] { "bc", "bb", "cd" }, "cd", new string[0]);
        }

        [TestMethod]
        public void AfterLastWord()
        {
            SuccessorsTestHelper(new[] { "bc", "bb", "cd" }, "cde", new string[0]);
        }

        [TestMethod]
        public void AfterLastWord2()
        {
            SuccessorsTestHelper(new[] { "bc", "bb", "cd" }, "dd", new string[0]);
        }        
    }
}
