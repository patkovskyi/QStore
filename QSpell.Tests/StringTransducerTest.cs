using QSpell.Transducer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using QSpell.Comparers;

namespace QSpell.Tests
{
    [TestClass()]
    public class StringTransducerTest
    {
        public void StringTransducerCreateTestHelper(IEnumerable<Tuple<string, float, string>> rules, char emptyChar)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var shuffledRules = rules.OrderBy(s => rand.NextDouble());

            var newRules = shuffledRules.Select(r =>
            {
                Int32 width = Math.Max(r.Item1.Length, r.Item3.Length);
                var left = r.Item1.PadRight(width, emptyChar);
                var right = r.Item3.PadRight(width, emptyChar);
                var tuples = new Tuple<char, char>[width];
                for (int i = 0; i < width; i++)
                {
                    tuples[i] = new Tuple<char, char>(left[i], right[i]);
                }
                return new Tuple<IEnumerable<Tuple<char, char>>, float>(tuples, r.Item2);
            }).ToArray();

            var target = StringTransducer.Create(rules, emptyChar);

            var comparer = new RuleComparer<char, char>(Comparer<char>.Default, Comparer<char>.Default);
            var expectedSequences = newRules.OrderBy(s => s, comparer).ToArray();
            var actualSequences = target.ToArray();
            CollectionAssert.AreEqual(expectedSequences, actualSequences, comparer);
        }

        [TestMethod()]
        public void StringTransducerCreateTest1()
        {
            var rules = new Tuple<string, float, string>[]
            {
                new Tuple<string, float, string>("abc", 1.1F, "abb"),
                new Tuple<string, float, string>("s", 0.33F, "ss"),
                new Tuple<string, float, string>("i", 0.66F, "e"),
                new Tuple<string, float, string>("i", 0.5F, "ie"),
            };
            StringTransducerCreateTestHelper(rules, '#');
        }
    }
}
