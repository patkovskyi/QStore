namespace QStore.Tests.QIndexedStringSetTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QStore.Tests.Comparers;
   
    [TestClass]
    public class GetByIndexTests
    {
        public static void GetByIndexTestHelper(IComparer<char> comparer, params string[] strings)
        {            
            var target = QIndexedStringSet.Create(strings, comparer);
            var sequenceComparer = new SequenceComparer<char>(comparer);
            var expected = strings.OrderBy(s => s, sequenceComparer).ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], target.GetByIndex(i));
            }
        }

        public static void GetByIndexTestHelper(params string[] strings)
        {
            GetByIndexTestHelper(Comparer<char>.Default, strings);
        }

        [TestMethod]
        public void GetByIndexSimple1()
        {            
            GetByIndexTestHelper("aa", "ab", "ac", "abc");
        }

        [TestMethod]
        public void GetByIndexSimple2()
        {
            GetByIndexTestHelper("one", "two", "three", "four", "five");
        }    
    }
}
