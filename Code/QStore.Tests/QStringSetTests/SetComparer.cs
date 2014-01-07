namespace QStore.Tests.QStringSetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.TestHelpers;

    [TestClass]
    public class SetComparer
    {
        private class WeirdComparer : IComparer<char>
        {
            private int Counter { get; set; }
            private int WeirdIndex { get; set; }

            public WeirdComparer(int weirdIndex)
            {
                this.WeirdIndex = weirdIndex;
            }

            public int Compare(char x, char y)
            {
                int comp = Comparer<char>.Default.Compare(x, y);
                if (Counter++ != WeirdIndex) return comp;
                return -comp;
            }
        }

        [TestMethod]
        public void SetComparerOk()
        {
            var words = new[] { "aa", string.Empty, "ab", "ac", "abc" };
            var initComparer = Comparer<char>.Default;            
            var set = QStringSet.Create(words, initComparer);

            var newComparer = Comparer<char>.Default;
            set.SetComparer(newComparer, true);

            var expected = words.OrderBy(w => w).ToArray();
            var actual = set.Enumerate().ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetComparerNull()
        {
            var words = new[] { "aa", string.Empty, "ab", "ac", "abc" };
            var initComparer = Comparer<char>.Default;
            var set = QStringSet.Create(words, initComparer);

            var e = ExceptionAssert.Throws<ArgumentNullException>(() => set.SetComparer(null));
            Assert.AreEqual("comparer", e.ParamName);
        }

        [TestMethod]
        public void SetComparerWrongCheck()
        {
            var words = new[] { "aa", string.Empty, "ab", "ac", "abc" };
            var initComparer = Comparer<char>.Default;
            var set = QStringSet.Create(words, initComparer);

            var e = ExceptionAssert.Throws<ArgumentException>(() => set.SetComparer(new WeirdComparer(1), true));
            Assert.AreEqual("comparer", e.ParamName);
            StringAssert.Contains(e.Message, ErrorMessages.ComparerMismatch);
        }

        [TestMethod]
        public void SetComparerWrongDontCheck()
        {
            var words = new[] { "aa", string.Empty, "ab", "ac", "abc" };
            var initComparer = Comparer<char>.Default;
            var set = QStringSet.Create(words, initComparer);
            set.SetComparer(new WeirdComparer(1));             
        }
    }
}