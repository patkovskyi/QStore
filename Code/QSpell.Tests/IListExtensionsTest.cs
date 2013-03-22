using QSpell.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using QSpell.Spellchecker;
using QSpell.Comparers;

namespace QSpell.Tests
{
    [TestClass()]
    public class IListExtensionsTest
    {
        [TestMethod()]
        public void LinearSearchTest1()
        {
            IList<StringRule> list = new List<StringRule>() 
            { 
                new StringRule("a", 1, "b"),
                new StringRule("ab", 1, "bb"),
                new StringRule("aa", 1, "aa"),
                new StringRule("", 1, "a"),
            };
            StringRule item = new StringRule("a", 0, "a");
            IComparer<StringRule> comparer = new RuleCostComparer<char, char>();
            int lower = 0;
            int upper = 4;
            int expected = list.BinarySearch(item, comparer);
            int actual;
            actual = IListExtensions.LinearSearch<StringRule>(list, item, comparer, lower, upper);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LinearSearchTest2()
        {
            List<StringRule> list = new List<StringRule>() 
            { 
                new StringRule("a", 0, "b"),
                new StringRule("ab", 0, "bb"),
                new StringRule("aa", 0, "aa"),
                new StringRule("", 0, "a"),
            };
            StringRule item = new StringRule("a", 1, "a");
            IComparer<StringRule> comparer = new RuleCostComparer<char, char>();
            int lower = 0;
            int upper = 4;
            int expected = list.BinarySearch(item, comparer);
            int actual;
            actual = IListExtensions.LinearSearch<StringRule>(list, item, comparer, lower, upper);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void BinarySearchTest1()
        {
            IList<StringRule> list = new List<StringRule>() 
            { 
                new StringRule("a", 1, "b"),
                new StringRule("ab", 1, "bb"),
                new StringRule("aa", 1, "aa"),
                new StringRule("", 1, "a"),
            };
            StringRule item = new StringRule("a", 0, "a");
            IComparer<StringRule> comparer = new RuleCostComparer<char, char>();
            int lower = 0;
            int upper = 4;
            int expected = list.BinarySearch(item, comparer);
            int actual;
            actual = IListExtensions.BinarySearch<StringRule>(list, item, comparer, lower, upper);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void BinarySearchTest2()
        {
            List<StringRule> list = new List<StringRule>() 
            { 
                new StringRule("a", 0, "b"),
                new StringRule("ab", 0, "bb"),
                new StringRule("aa", 0, "aa"),
                new StringRule("", 0, "a"),
            };
            StringRule item = new StringRule("a", 1, "a");
            IComparer<StringRule> comparer = new RuleCostComparer<char, char>();
            int lower = 0;
            int upper = 4;
            int expected = list.BinarySearch(item, comparer);
            int actual;
            actual = IListExtensions.BinarySearch<StringRule>(list, item, comparer, lower, upper);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SortedInsertTest1()
        {
            IList<StringRule> list = new List<StringRule>() 
            { 
                new StringRule("a", 1, "b"),
                new StringRule("ab", 1, "bb"),
                new StringRule("aa", 1, "aa"),
                new StringRule("", 1, "a"),
            };
            StringRule item = new StringRule("a", 0, "a");
            IComparer<StringRule> comparer = new RuleCostComparer<char, char>();
            int lower = 0;
            int upper = 4;
            int expected = 0;
            int actual;
            actual = IListExtensions.SortedInsert<StringRule>(ref list, item, comparer, lower, upper);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(item, list[0]);
            Assert.AreEqual(5, list.Count);
        }

        [TestMethod()]
        public void SortedInsertTest2()
        {
            IList<StringRule> list = new List<StringRule>() 
            { 
                new StringRule("a", 0, "b"),
                new StringRule("ab", 0, "bb"),
                new StringRule("aa", 0, "aa"),
                new StringRule("", 0, "a"),
            };
            StringRule item = new StringRule("a", 1, "a");
            IComparer<StringRule> comparer = new RuleCostComparer<char, char>();
            int lower = 0;
            int upper = 4;
            int expected = 4;
            int actual;
            actual = IListExtensions.SortedInsert<StringRule>(ref list, item, comparer, lower, upper);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(item, list[4]);
            Assert.AreEqual(5, list.Count);
        }

        [TestMethod()]
        public void SortedInsertTest3()
        {
            IList<StringRule> list = new List<StringRule>() 
            { 
                new StringRule("a", 0, "a"),
            };
            StringRule item = new StringRule("a", 1, "b");
            IComparer<StringRule> comparer = new RuleCostComparer<char, char>();            
            int expected = 1;
            int actual;
            actual = IListExtensions.SortedInsert<StringRule>(ref list, item, comparer);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(item, list[1]);
            Assert.AreEqual(2, list.Count);
        }
    }
}
