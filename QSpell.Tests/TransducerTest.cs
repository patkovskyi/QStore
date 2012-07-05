using QSpell.Transducer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QSpell.Tests
{
    [TestClass()]
    public class TransducerTest
    {
        public void TransducerCreateTestHelper<I, O>(IEnumerable<Tuple<IEnumerable<Tuple<I, O>>, float>> rules, IComparer<I> inputComparer, IComparer<O> outputComparer)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var shuffledRules = rules.OrderBy(s => rand.NextDouble());

            var target = Transducer<I, O>.Create(rules, inputComparer, outputComparer);

            var comparer = new RuleComparer<I, O>(inputComparer, outputComparer);
            var expectedSequences = rules.OrderBy(s => s, comparer).ToArray();
            var actualSequences = target.ToArray();
            CollectionAssert.AreEqual(expectedSequences, actualSequences, comparer);
        }

        [TestMethod()]
        public void TransducerCreateTest1()
        {            
            //CreateTestHelper();
        }
    }
}
