using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using QSpell.Helpers;

namespace QSpell.Playground
{
    class Program
    {
        /// <summary>
        /// All_Forms.txt can be downloaded here: http://speakrus.ru/dict/all_forms.rar
        /// </summary>
        static void PrepareZaliznyak()
        {
            string oldFileName = "All_Forms.txt";
            string newFileName = "Zaliznyak.txt";

            string[] oldLines = File.ReadAllLines(oldFileName, Encoding.GetEncoding(1251));
            var newLines = new List<string>();
            for (int i = 0; i < oldLines.Length; i++)
            {
                string oldLine = oldLines[i];
                int sharpIndex = oldLine.IndexOf("#");
                var sb = new StringBuilder();
                for (int j = sharpIndex + 1; j < oldLine.Length; j++)
                {
                    switch (oldLine[j])
                    {
                        case '\'':
                        case '`':
                            break;
                        case ',':
                            newLines.Add(sb.ToString());
                            sb.Clear();
                            break;
                        default:
                            sb.Append(oldLine[j]);
                            break;
                    }
                }
                newLines.Add(sb.ToString());
            }

            File.WriteAllLines(newFileName, newLines.Distinct(StringComparer.Ordinal).OrderBy(s => s, StringComparer.Ordinal), Encoding.GetEncoding(1251));
        }

        static void Main(string[] args)
        {
            IEnumerable<IEnumerable<char>> values = new string[]{"world", "boy", "girl"};
            var bytes = ProtoBufHelper.SerializeAsBytes(new Foo<char>("hello", 42, values));
            var message = ProtoBufHelper.DeserializeFromBytes<Foo<char>>(bytes);
        }
    }
}
