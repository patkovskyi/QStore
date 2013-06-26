using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QSpell.Playground
{
    using System.Diagnostics;

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

        static void TestStringConversion()
        {            
            IEnumerable<IEnumerable<char>> strings = Enumerable.Repeat(0, 1000000).Select(i => Path.GetRandomFileName());
            var watch = new Stopwatch();
            var ctorStrings = strings.Select(seq => new string(seq.ToArray())).ToArray();
            watch.Start();
            ctorStrings = strings.Select(seq => new string(seq.ToArray())).ToArray();
            watch.Stop();
            Console.WriteLine("CtorStrings: {0}", watch.Elapsed);

            var concatStrings = strings.Select(seq => string.Concat(seq)).ToArray();
            watch.Restart();
            concatStrings = strings.Select(seq => string.Concat(seq)).ToArray();
            watch.Stop();
            Console.WriteLine("ConcatStrings: {0}", watch.Elapsed);
        }

        private static void Shuffle(string originalPath, string outputPath, Encoding encoding)
        {            
            var r = new Random();
            File.WriteAllLines(
                outputPath,
                File.ReadAllLines(originalPath, encoding).OrderBy(line => r.Next()).ToArray(),
                encoding);
        }

        static void Main(string[] args)
        {
            var tester = PerformanceTester.Create("Zaliznyak-1251.txt", Encoding.GetEncoding(1251));
            tester.TestVsSortedDictionary();

            //Shuffle("Zaliznyak-baseforms-1251.txt", "Zaliznyak-baseforms-1251.txt", Encoding.GetEncoding(1251));
            //Shuffle("Zaliznyak-1251.txt", "Zaliznyak-1251.txt", Encoding.GetEncoding(1251));
            //TestStringConversion();
            //var before = GC.GetTotalMemory(true);
            //var pairs = new AlignedPair<int, byte>[1000];
            //var after = GC.GetTotalMemory(true);
            //Console.WriteLine("+{0}", after - before);
        }
    }
}
