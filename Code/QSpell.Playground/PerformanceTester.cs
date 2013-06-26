namespace QSpell.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    using QStore.Strings;

    public class PerformanceTester
    {
        private Dictionary<string, int> dictionary;

        private QStringMap<int> map;

        private SortedDictionary<string, int> sortedDictionary;

        private SortedList<string, int> sortedList;

        private string[] words;

        public static PerformanceTester Create(string fromFile, Encoding encoding)
        {
            var tester = new PerformanceTester { words = File.ReadAllLines(fromFile, encoding) };            
            return tester;
        }

        public int TestVsDictionary()
        {            
            InitMap();
            InitDictionary();

            Console.WriteLine(@"Testing map vs dictionary");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int x = 0;
            foreach (var word in words)
            {
                x = dictionary[word];
            }            
            stopwatch.Stop();
            Console.WriteLine(@"Dictionary time for {0} words: {1} ms", words.Length, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            foreach (var word in words)
            {
                x = map[word];
            }
            stopwatch.Stop();
            Console.WriteLine(@"       Map time for {0} words: {1} ms", words.Length, stopwatch.ElapsedMilliseconds);
            Console.WriteLine();
            return x;
        }

        public int TestVsSortedDictionary()
        {
            InitMap();
            InitSortedDictionary();

            Console.WriteLine(@"Testing map vs sorted dictionary");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int x = 0;
            foreach (var word in words)
            {
                x = sortedDictionary[word];
            }
            stopwatch.Stop();
            Console.WriteLine(@"Sorted dictionary time for {0} words: {1} ms", words.Length, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            foreach (var word in words)
            {
                x = map[word];
            }
            stopwatch.Stop();
            Console.WriteLine(@"              Map time for {0} words: {1} ms", words.Length, stopwatch.ElapsedMilliseconds);
            Console.WriteLine();
            return x;
        }

        private void InitMap()
        {
            if (map == null)
            {
                Console.WriteLine(@"Initializing QStringMap<int> with {0} elements.", words.Length);
                var stopwatch = new Stopwatch();
                long memoryBefore = GetCurrentUsedMemory();
                stopwatch.Start();                

                map = QStringMap<int>.Create(words, Comparer<char>.Default);
                foreach (var word in words)
                {
                    map[word] = word.GetHashCode();
                }

                stopwatch.Stop();
                long memoryAfter = GetCurrentUsedMemory();
                Console.WriteLine(@"Memory delta: {0:+#;-#} bytes", memoryAfter - memoryBefore);
                Console.WriteLine(@"Elapsed time:  {0} ms", stopwatch.ElapsedMilliseconds);
                Console.WriteLine();
            }
        }

        private void InitDictionary()
        {
            if (dictionary == null)
            {
                Console.WriteLine(@"Initializing Dictionary<string, int> with {0} elements.", words.Length);
                var stopwatch = new Stopwatch();
                long memoryBefore = GetCurrentUsedMemory();
                stopwatch.Start();                

                dictionary = words.ToDictionary(w => w, w => w.GetHashCode());

                stopwatch.Stop();
                long memoryAfter = GetCurrentUsedMemory();
                Console.WriteLine(@"Memory delta: {0:+#;-#} bytes", memoryAfter - memoryBefore);
                Console.WriteLine(@"Elapsed time:  {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }

        private void InitSortedDictionary()
        {
            if (sortedDictionary == null)
            {
                Console.WriteLine(@"Initializing SortedDictionary<string, int> with {0} elements.", words.Length);
                var stopwatch = new Stopwatch();
                long memoryBefore = GetCurrentUsedMemory();
                stopwatch.Start();               

                sortedDictionary = new SortedDictionary<string, int>();
                foreach (var word in words)
                {
                    sortedDictionary.Add(word, word.GetHashCode());
                }

                stopwatch.Stop();
                long memoryAfter = GetCurrentUsedMemory();
                Console.WriteLine(@"Memory delta: {0:+#;-#} bytes", memoryAfter - memoryBefore);
                Console.WriteLine(@"Elapsed time:  {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }

        private static long GetCurrentUsedMemory()
        {
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            return GC.GetTotalMemory(true);
        }
    }
}