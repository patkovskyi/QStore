using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace QSpell.Spellchecker
{
    public class StringRule : Rule<char, char>
    {
        public string LeftStr { get { return new String(Left); } }
        public string RightStr { get { return new String(Right); } }

        public StringRule(string left, double cost, string right) : base(left.ToArray(), cost, right.ToArray()) { }
    }
}
