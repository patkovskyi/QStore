using QSpell.Spellchecker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using QSpell.Tests.Comparers;

namespace QSpell.Tests
{
    [TestClass()]
    public class QSpellcheckerTest
    {
        public static char[] ALPHABET = Enumerable.Range(0, 26).Select(i => (char)('a' + i)).Concat(Enumerable.Range(0, 26).Select(i => (char)('A' + i))).ToArray();

        public void GetCorrectionsTestHelper<T>(KeyValuePair<string, T>[] lexicon,
            StringRule[] rules, string input, int suggestionsLimit, double costLimit,
            Suggestion<T>[] expected)
            where T : IComparable<T>
        {
            QSpellchecker<T> target = new QSpellchecker<T>(rules, lexicon);
            int timeLimit = 0;
            bool limitToBestPaths = false;
            Nullable<CancellationToken> token = new Nullable<CancellationToken>();
            Suggestion<T>[] actual = target.GetCorrections(input, timeLimit, suggestionsLimit, costLimit, limitToBestPaths, token).ToArray();
            CollectionAssert.AreEqual(expected, actual, new SuggestionComparer<T>());
        }

        [TestMethod()]
        public void GetCorrectionsTest1()
        {
            var lexicon = new KeyValuePair<string, byte>[]
            {
                new KeyValuePair<string, byte>("defied", 3),
                new KeyValuePair<string, byte>("defies", 4),
                new KeyValuePair<string, byte>("defy", 0),
                new KeyValuePair<string, byte>("defying", 235),
                new KeyValuePair<string, byte>("denied", 111),
                new KeyValuePair<string, byte>("denies", 13),
                new KeyValuePair<string, byte>("deny", 14),
                new KeyValuePair<string, byte>("denying", 15),
                new KeyValuePair<string, byte>("trie", 15),
                new KeyValuePair<string, byte>("tried", 16),
                new KeyValuePair<string, byte>("tries", 14),
                new KeyValuePair<string, byte>("try", 21),
                new KeyValuePair<string, byte>("trying", 22),
            };
            
            var rules = (new LevenshteinModel(new String(ALPHABET))).GetRules().ToArray();
            var input = "denyn";
            int suggestionsLimit = 3;
            double costLimit = 0;
            var expected = new Suggestion<byte>[]
            {
                new Suggestion<byte>("deny", 1, 14),
                new Suggestion<byte>("denying", 2, 15),
                new Suggestion<byte>("defy", 2, 0),
            };

            GetCorrectionsTestHelper(lexicon, rules, input, suggestionsLimit, costLimit, expected);
        }
    }
}
