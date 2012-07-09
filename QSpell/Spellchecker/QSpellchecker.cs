using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using QSpell.Comparers;
using QSpell.Sequences;

namespace QSpell.Spellchecker
{
    public class QSpellchecker<T>
        where T : IComparable<T>
    {
        protected SequenceDictionary<char, T> _lexicon;
        protected Dictionary<string, StringRule[]> _ruleCache;
        protected int _maxPrefixLength;

        public QSpellchecker(IEnumerable<StringRule> rules, SequenceDictionary<char, T> lexicon)
        {
            InitRules(rules);
            _lexicon = lexicon;
        }

        public QSpellchecker(IEnumerable<StringRule> rules, IEnumerable<KeyValuePair<string, T>> lexicon)
        {
            InitRules(rules);
            _lexicon = SequenceDictionary<char, T>.Create(lexicon.Select(l => new KeyValuePair<IEnumerable<char>, T>(l.Key, l.Value)), Comparer<char>.Default, true);
        }

        public bool Contains(string s)
        {
            return _lexicon.ContainsSequence(s);
        }

        public IEnumerable<Suggestion<T>> GetCorrections(
            string input, int timeLimit = 1000, int suggestionsLimit = 0, double costLimit = 0, bool limitToBestPaths = false, CancellationToken? token = null)
        {
            return GetCorrectionsProtected(input: input, timeLimit: timeLimit, costLimit: costLimit, suggestionsLimit: suggestionsLimit,
            limitToBestPaths: limitToBestPaths, token: token);
        }

        public IEnumerable<Suggestion<T>> GetFrequencyRankedCorrections(
            string input, int timeLimit = 1000, int suggestionsLimit = 0, double costLimit = 0, bool limitToBestPaths = false, CancellationToken? token = null)
        {
            return GetCorrectionsProtected(input: input, timeLimit: timeLimit, costLimit: costLimit, suggestionsLimit: suggestionsLimit,
            limitToBestPaths: limitToBestPaths, token: token).OrderByDescending(s => s.Frequency).ThenBy(s => s.Cost);
        }

        protected void InitRules(IEnumerable<StringRule> rules)
        {
            var prefixTree = new PrefixTree<StringRule>(new RuleCostComparer<char, char>());
            foreach (var rule in rules)
            {
                prefixTree.Add(new string(rule.Left), rule);
            }
            var prefixes = rules.Select(e => new string(e.Left)).Distinct(StringComparer.Ordinal);
            _ruleCache = new Dictionary<string, StringRule[]>();
            _maxPrefixLength = prefixes.Max(p => p.Length);
            foreach (var prefix in prefixes)
            {
                _ruleCache.Add(prefix, prefixTree.Get(prefix).ToArray());
            }
        }

        protected StringRule[] Filter(string input, int fromIndex)
        {
            for (int i = Math.Min(_maxPrefixLength, input.Length - fromIndex); i >= 0; i--)
            {
                StringRule[] res = null;
                if (_ruleCache.TryGetValue(input.Substring(fromIndex, i), out res))
                {
                    return res;
                }
            }
            return new StringRule[0];
        }

        protected IEnumerable<Suggestion<T>> GetCorrectionsProtected(string input, int timeLimit, double costLimit, int suggestionsLimit, bool limitToBestPaths, CancellationToken? token)
        {
            var stack = new PriorityStack<double, QStackObject<char, char>>();
            var startStackObject = new QStackObject<char, char>(_lexicon.Start, false, 0, 0, null, null, null, 0);
            stack.Push(0, startStackObject);
            var returned = new HashSet<string>();

            double bestCost = double.MaxValue;
            Stopwatch watch = null;
            if (timeLimit != 0)
            {
                watch = new Stopwatch();
                watch.Start();
            }
            while (!stack.IsEmpty)
            {
                var top = stack.Pop();

                // stop conditions
                if (timeLimit != 0 && watch.ElapsedMilliseconds > timeLimit ||
                    costLimit != 0 && top.Cost > costLimit ||
                    limitToBestPaths && top.Cost > bestCost ||
                    suggestionsLimit != 0 && returned.Count >= suggestionsLimit ||
                    token.HasValue && token.Value.IsCancellationRequested)
                {
                    yield break;
                }

                if (top.IsFinal && top.CharIndex >= input.Length)
                {
                    string output = new string(top.Backtrace().ToArray());
                    if (!returned.Contains(output))
                    {
                        if (limitToBestPaths && bestCost == double.MaxValue)
                        {
                            bestCost = top.Cost;
                        }
                        returned.Add(output);
                        yield return new Suggestion<T>(output, top.Cost, _lexicon[output]);
                    }
                    continue;
                }

                var filteredRules = top.CachedRules ?? (top.CachedRules = Filter(input, top.CharIndex));
                for (int i = top.LastRuleIndex; i < filteredRules.Length; )
                {
                    var rule = filteredRules[i++];
                    SequenceSetTransition nextTransition;
                    if (_lexicon.TrySend(top.State, rule.Right, out nextTransition))
                    {
                        top.LastRuleIndex = i;
                        stack.Push(top.Cost + rule.Cost, top);
                        QStackObject<char, char> newStackObject = null;
                        if (rule.Right.Length == 0)
                        {
                            // Fix for case when TrySend returns default(SequenceSetTransition)
                            newStackObject = new QStackObject<char, char>(top.State, top.IsFinal, top.Cost + rule.Cost, top.CharIndex + rule.Left.Length, top, rule, null, 0);
                        }
                        else
                        {
                            newStackObject = new QStackObject<char, char>(nextTransition.StateIndex, nextTransition.IsFinal, top.Cost + rule.Cost, top.CharIndex + rule.Left.Length, top, rule, null, 0);
                        }
                        stack.Push(top.Cost + rule.Cost, newStackObject);
                        break;
                    }
                }
            }
        }
    }
}
