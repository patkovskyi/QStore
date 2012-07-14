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
        public static char[] ENGLISH_ALPHABET = Enumerable.Range(0, 26).Select(i => (char)('a' + i)).Concat(Enumerable.Range(0, 26).Select(i => (char)('A' + i))).ToArray();

        public static char[] RUSSIAN_ALPHABET = " ,-абвгдежзийклмнопрстуфхцчшщъыьэюя".ToArray();

        public void GetCorrectionsTestHelper<T>(KeyValuePair<string, T>[] lexicon,
            StringRule[] rules, string input, int suggestionsLimit, double costLimit, double autoCompletionRate,
            Suggestion<T>[] expected)
            where T : IComparable<T>
        {
            QSpellchecker<T> target = new QSpellchecker<T>(rules, lexicon);
            int timeLimit = 0;
            bool limitToBestPaths = false;
            Nullable<CancellationToken> token = new Nullable<CancellationToken>();
            Suggestion<T>[] actual = target.GetCorrections(input, timeLimit, costLimit, limitToBestPaths, token, autoCompletionRate).Take(suggestionsLimit).ToArray();
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

            var rules = (new LevenshteinModel(new String(ENGLISH_ALPHABET))).GetRules().ToArray();
            var input = "denyn";
            int suggestionsLimit = 3;
            double costLimit = 0;
            double autoCompletionRate = 1.0;
            var expected = new Suggestion<byte>[]
            {
                new Suggestion<byte>("deny", 1, 14),
                new Suggestion<byte>("denying", 2, 15),
                new Suggestion<byte>("defy", 2, 0),
            };

            GetCorrectionsTestHelper(lexicon, rules, input, suggestionsLimit, costLimit, autoCompletionRate, expected);
        }

        [TestMethod()]
        public void GetCorrectionsTest2()
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

            var rules = (new LevenshteinModel(new String(ENGLISH_ALPHABET))).GetRules().ToArray();
            var input = "dem";
            int suggestionsLimit = 3;
            double costLimit = 0;
            double autoCompletionRate = 0.1;
            var expected = new Suggestion<byte>[]
            {
                new Suggestion<byte>("deny", 1.1, 14),
                new Suggestion<byte>("defy", 1.1, 0),
                new Suggestion<byte>("denies", 1.3, 13),
            };

            GetCorrectionsTestHelper(lexicon, rules, input, suggestionsLimit, costLimit, autoCompletionRate, expected);
        }

        [TestMethod()]
        public void GetCorrectionsTest3()
        {
            var lexicon = new KeyValuePair<string, byte>[]
            {
                new KeyValuePair<string, byte>("молок", 3),
                new KeyValuePair<string, byte>("молоко", 0),
                new KeyValuePair<string, byte>("мал", 111),
                new KeyValuePair<string, byte>("малого", 13),
                new KeyValuePair<string, byte>("милок", 14),
            };

            var rules = (new LevenshteinModel(new String(RUSSIAN_ALPHABET))).GetRules().ToArray();
            var input = "молок";
            int suggestionsLimit = 2;
            double costLimit = 0;
            double autoCompletionRate = 0.1;
            var expected = new Suggestion<byte>[]
            {
                new Suggestion<byte>("молок", 0, 3),
                new Suggestion<byte>("молоко", 0.1, 0),
            };

            GetCorrectionsTestHelper(lexicon, rules, input, suggestionsLimit, costLimit, autoCompletionRate, expected);
        }
    }
}
