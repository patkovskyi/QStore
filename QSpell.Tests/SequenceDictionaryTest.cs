using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QSpell.Comparers;
using QSpell.Sequences;
using QSpell.Helpers;

namespace QSpell.Tests
{
    [TestClass()]
    public class SequenceDictionaryTest
    {
        public SequenceDictionary<T, V> CreateTestHelper<T, V>(IEnumerable<KeyValuePair<IEnumerable<T>, V>> sequences, IComparer<T> comparer, IComparer<V> valueComparer)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var shuffledSequences = sequences.OrderBy(s => rand.NextDouble());

            var target = SequenceDictionary<T, V>.Create(shuffledSequences, comparer, false);

            target.Minimize();

            var bytes = target.Serialize();
            target = SequenceDictionary<T, V>.Deserialize(bytes, comparer);

            var sequenceComparer = new SequenceKeyValueComparer<T, V>(comparer, valueComparer);
            var expectedSequences = sequences.OrderBy(s => s, sequenceComparer).ToArray();
            var actualSequences = (target as IEnumerable<KeyValuePair<IEnumerable<T>, V>>).ToArray();
            CollectionAssert.AreEqual(expectedSequences, actualSequences, sequenceComparer);

            return target;
        }

        [TestMethod()]
        public void CreateTest1()
        {
            var sequences = new List<KeyValuePair<IEnumerable<char>, byte>>() 
            { 
                new KeyValuePair<IEnumerable<char>, byte>("tap", 255), 
                new KeyValuePair<IEnumerable<char>, byte>("taps", 254), 
                new KeyValuePair<IEnumerable<char>, byte>("top", 253), 
                new KeyValuePair<IEnumerable<char>, byte>("tops", 252), 
            };
            CreateTestHelper(sequences, Comparer<char>.Default, Comparer<byte>.Default);
        }

        [TestMethod()]
        public void CreateTest2()
        {
            var sequences = File.ReadAllLines(@"..\..\..\TestData\Baseforms.txt", Encoding.GetEncoding(1251)).Select(s => new KeyValuePair<IEnumerable<char>, double>(s, s.GetHashCode() / (double)s.Length)).ToArray();
            CreateTestHelper(sequences, Comparer<char>.Default, Comparer<double>.Default);
        }

        [TestMethod()]
        public void CreateTest3()
        {
            var sequences = File.ReadAllLines(@"..\..\..\TestData\Zaliznyak.txt", Encoding.GetEncoding(1251)).Select(s => new KeyValuePair<IEnumerable<char>, byte>(s, (byte)(s.GetHashCode() % 256))).ToArray();
            CreateTestHelper(sequences, Comparer<char>.Default, Comparer<byte>.Default);
        }
    }
}
