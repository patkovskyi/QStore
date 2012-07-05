using QSpell.Sequences;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QSpell.Comparers;

namespace QSpell.Tests
{
    [TestClass()]
    public class SequenceSetTest
    {
        public void MinimizeTestHelper<T>(IEnumerable<IEnumerable<T>> sequences, IComparer<T> comparer,
            int expectedStartStateCount, int expectedStartTransitionsCount,
            int expectedEndStateCount, int expectedEndTransitionsCount)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            IEnumerable<IEnumerable<T>> shuffledSequences = sequences.OrderBy(s => rand.NextDouble());

            SequenceSet<T> target = SequenceSet<T>.Create(shuffledSequences, comparer, false);

            int actualStartStateCount = target.GetStatesCount();
            int actualStartTransitionsCount = target.GetTransitionsCount();
            Assert.AreEqual(expectedStartStateCount, actualStartStateCount);
            Assert.AreEqual(expectedStartTransitionsCount, actualStartTransitionsCount);

            target.Minimize();

            int actualEndStateCount = target.GetStatesCount();
            int actualEndTransitionsCount = target.GetTransitionsCount();
            Assert.AreEqual(expectedEndStateCount, actualEndStateCount);
            Assert.AreEqual(expectedEndTransitionsCount, actualEndTransitionsCount);

            var expectedSequences = sequences.OrderBy(s => s, new SequenceComparer<T>(comparer)).ToArray();
            var actualSequences = target.ToArray();
            CollectionAssert.AreEqual(expectedSequences, actualSequences, new SequenceComparer<T>(comparer));
        }

        [TestMethod()]
        public void MinimizeTest1()
        {
            MinimizeTestHelper(new String[] { "tap", "taps", "top", "tops" },
                Comparer<Char>.Default, 8, 7, 5, 5);
        }

        [TestMethod()]
        public void MinimizeTest2()
        {
            MinimizeTestHelper(File.ReadAllLines(@"..\..\..\TestData\Baseforms.txt", Encoding.GetEncoding(1251)),
                Comparer<Char>.Default, 357827, 357826, 49040, 108176);
        }

        [TestMethod()]
        public void MinimizeTest3()
        {
            MinimizeTestHelper(File.ReadAllLines(@"..\..\..\TestData\Zaliznyak.txt", Encoding.GetEncoding(1251)),
                Comparer<Char>.Default, 2531993, 2531992, 64810, 196694);
        }
    }
}
