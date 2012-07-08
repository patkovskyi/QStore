using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSpell.Spellchecker;

namespace QSpell
{
    public class LevenshteinModel
    {        
        protected List<StringRule> _errors;
        public LevenshteinModel(string alphabet)
        {
            _errors = new List<StringRule>();

            // no changes
            foreach (var cin in alphabet)
            {
                _errors.Add(new StringRule(cin.ToString(), 0, cin.ToString()));
            }

            // add
            foreach (var cin in alphabet)
            {
                _errors.Add(new StringRule(String.Empty, 1, cin.ToString()));
            }

            // delete
            foreach (var cin in alphabet)
            {
                _errors.Add(new StringRule(cin.ToString(), 1, String.Empty));
            }

            // change
            foreach (var cin in alphabet)
            {
                foreach (var cout in alphabet)
                {
                    _errors.Add(new StringRule(cin.ToString(), 1, cout.ToString()));
                }
            }

            _errors.TrimExcess();
        }

        public IEnumerable<StringRule> GetRules()
        {
            return _errors;
        }
    }
}
