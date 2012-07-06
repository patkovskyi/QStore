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
        protected Dictionary<String, Rule[]> _ruleCache;
        protected int _maxPrefixLength;

        protected Rule[] Filter(String input, int fromIndex)
        {
            for (int i = Math.Min(_maxPrefixLength, input.Length); i >= 0; i--)
            {
                Rule[] res = null;
                if (_ruleCache.TryGetValue(input.Substring(0, i), out res))
                {
                    return res;
                }
            }
            return new Rule[0];
        }

        public QSpellchecker(IEnumerable<Rule> rules, IEnumerable<KeyValuePair<string, T>> lexicon)
        {
            var prefixTree = new PrefixTree<Rule>(new RuleCostComparer());
            foreach (var rule in rules)
            {
                prefixTree.Add(rule.Left, rule);
            }
            var prefixes = rules.Select(e => e.Left).Distinct(StringComparer.Ordinal);
            _ruleCache = new Dictionary<string, Rule[]>();
            _maxPrefixLength = prefixes.Max(p => p.Length);
            foreach (var prefix in prefixes)
            {
                _ruleCache.Add(prefix, prefixTree.Get(prefix).ToArray());
            }
            _lexicon = SequenceDictionary<char, T>.Create(lexicon.Select(l => new KeyValuePair<IEnumerable<char>, T>(l.Key, l.Value)), Comparer<char>.Default, true);
        }

        public bool Contains(String s)
        {
            return _lexicon.ContainsSequence(s);
        }        

        protected IEnumerable<Suggestion<T>> GetCorrectionsProtected(String input, int timeLimit, double costLimit, int suggestionsLimit, bool limitToBestPaths, CancellationToken? token)
        {
            var stack = new PriorityStack<double, QStackObject>();
            var startStackObject = new QStackObject(_lexicon.Start, false, 0, 0, null, null, null, 0);            
            stack.Push(0, startStackObject);
            var returned = new HashSet<String>();

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

                if (top.IsFinal && top.CharIndex > input.Length)
                {
                    string output = top.Backtrace(input);
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

                var filteredRules = Filter(input, top.CharIndex);
                for (int i = top.LastRuleIndex; i < filteredRules.Length; )
                {
                    var rule = filteredRules[i++];
                    SequenceSetTransition nextTransition;
                    if (_lexicon.TrySend(top.State, rule.Right, out nextTransition))
                    {                        
                        top.LastRuleIndex = i;
                        stack.Push(top.Cost + rule.Cost, top);
                        var newStackObject = new QStackObject(nextTransition.StateIndex, nextTransition.IsFinal, top.Cost + rule.Cost, top.CharIndex + rule.Left.Length, top, rule, filteredRules, 0);
                        stack.Push(top.Cost + rule.Cost, newStackObject);
                        break;
                    }
                }                
            }
        }        

        public IEnumerable<Suggestion<T>> GetCorrections(
            String word, int timeLimit = 1000, int suggestionsLimit = 0, double costLimit = 0, bool limitToBestPaths = false, CancellationToken? token = null)
        {
            return GetCorrectionsProtected(input: word, timeLimit: timeLimit, costLimit: costLimit, suggestionsLimit: suggestionsLimit,
            limitToBestPaths: limitToBestPaths, token: token);
        }
    }
}
