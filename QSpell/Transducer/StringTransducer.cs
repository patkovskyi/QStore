using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Transducer;

namespace QSpell.Transducer
{
    internal class StringTransducer : Transducer<char, char>
    {
        public static StringTransducer Create(IEnumerable<Tuple<string, float, string>> rules, char emptyChar)
        {
            var newRules = rules.Select(r =>
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

            return StringTransducer.Create<StringTransducer>(newRules, Comparer<char>.Default, Comparer<char>.Default);
        }

        public char EmptyChar { get; protected set; }
    }
}
