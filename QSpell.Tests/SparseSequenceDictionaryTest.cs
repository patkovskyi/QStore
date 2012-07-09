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
    public class SparseSequenceDictionaryTest
    {
        public SparseSequenceDictionary<T, V> CreateTestHelper<T, V>(IEnumerable<KeyValuePair<IEnumerable<T>, V>> sequences, IComparer<T> comparer, IComparer<V> valueComparer)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var shuffledSequences = sequences.OrderBy(s => rand.NextDouble());

            var target = SparseSequenceDictionary<T, V>.Create(shuffledSequences, comparer, false);

            target.Minimize();

            var bytes = target.Serialize();
            target = SparseSequenceDictionary<T, V>.Deserialize(bytes, comparer);

            var sequenceComparer = new SequenceKeyValueComparer<T, V>(comparer, valueComparer);
            var expectedSequences = sequences.OrderBy(s => s, sequenceComparer).ToArray();
            var actualSequences = (target as IEnumerable<KeyValuePair<IEnumerable<T>, V>>).ToArray();
            CollectionAssert.AreEqual(expectedSequences, actualSequences, sequenceComparer);

            return target;
        }

        public SparseSequenceDictionary<T, V> IndexerTestHelper<T, V>(IEnumerable<KeyValuePair<IEnumerable<T>, V>> sequences, IComparer<T> comparer, IComparer<V> valueComparer)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var shuffledSequences = sequences.OrderBy(s => rand.NextDouble());

            var target = SparseSequenceDictionary<T, V>.Create(shuffledSequences, comparer, true);

            var bytes = target.Serialize();
            target = SparseSequenceDictionary<T, V>.Deserialize(bytes, comparer);

            foreach (var sequence in sequences)
            {
                var expected = sequence.Value;
                var actual = target[sequence.Key];
                Assert.IsTrue(valueComparer.Compare(expected, actual) == 0);
            }

            return target;
        }

        [TestMethod()]
        public void SparseSequenceDictionaryCreateTest1()
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
        public void SparseSequenceDictionaryCreateTest2()
        {
            var sequences = File.ReadAllLines(@"..\..\..\TestData\Baseforms.txt", Encoding.GetEncoding(1251)).Select(s => new KeyValuePair<IEnumerable<char>, double>(s, s.GetHashCode() / (double)s.Length)).ToArray();
            CreateTestHelper(sequences, Comparer<char>.Default, Comparer<double>.Default);
        }

        [TestMethod()]
        public void SparseSequenceDictionaryCreateTest3()
        {
            var sequences = File.ReadAllLines(@"..\..\..\TestData\Zaliznyak.txt", Encoding.GetEncoding(1251)).Select((s, i) => new KeyValuePair<IEnumerable<char>, byte>(s, i > 69000 ? (byte)0 : (byte)(s.GetHashCode() % 256))).ToArray();
            CreateTestHelper(sequences, Comparer<char>.Default, Comparer<byte>.Default);
        }

        [TestMethod()]
        public void SparseSequenceDictionaryIndexerTest1()
        {
            var lexicon = new KeyValuePair<IEnumerable<char>, byte>[]
            {
                new KeyValuePair<IEnumerable<char>, byte>("defied", 3),
                new KeyValuePair<IEnumerable<char>, byte>("defies", 4),
                new KeyValuePair<IEnumerable<char>, byte>("defy", 0),
                new KeyValuePair<IEnumerable<char>, byte>("defying", 235),
                new KeyValuePair<IEnumerable<char>, byte>("denied", 111),
                new KeyValuePair<IEnumerable<char>, byte>("denies", 13),
                new KeyValuePair<IEnumerable<char>, byte>("deny", 14),
                new KeyValuePair<IEnumerable<char>, byte>("denying", 15),
                new KeyValuePair<IEnumerable<char>, byte>("trie", 15),
                new KeyValuePair<IEnumerable<char>, byte>("tried", 16),
                new KeyValuePair<IEnumerable<char>, byte>("tries", 14),
                new KeyValuePair<IEnumerable<char>, byte>("try", 21),
                new KeyValuePair<IEnumerable<char>, byte>("trying", 22),
            };

            var target = SparseSequenceDictionary<char, byte>.Create(lexicon, Comparer<char>.Default, true);
            //IndexerTestHelper(lexicon, Comparer<char>.Default, Comparer<byte>.Default);

            Assert.AreEqual(3, target["defied"]);
            Assert.AreEqual(4, target["defies"]);
            Assert.AreEqual(0, target["defy"]);
            Assert.AreEqual(235, target["defying"]);
            Assert.AreEqual(111, target["denied"]);
            Assert.AreEqual(13, target["denies"]);
            Assert.AreEqual(14, target["deny"]);
            Assert.AreEqual(15, target["denying"]);
            Assert.AreEqual(15, target["trie"]);
            Assert.AreEqual(16, target["tried"]);
            Assert.AreEqual(14, target["tries"]);
            Assert.AreEqual(21, target["try"]);
            Assert.AreEqual(22, target["trying"]);
        }

        [TestMethod()]
        public void SparseSequenceDictionaryIndexerTest2()
        {
            var lexicon = File.ReadAllLines(@"..\..\..\TestData\Baseforms.txt", Encoding.GetEncoding(1251)).Select(s => new KeyValuePair<IEnumerable<char>, byte>(s, (byte)(s.GetHashCode() % 256))).ToArray();
            var target = SparseSequenceDictionary<char, byte>.Create(lexicon, Comparer<char>.Default, true);
            IndexerTestHelper(lexicon, Comparer<char>.Default, Comparer<byte>.Default);
        }
    }
}
