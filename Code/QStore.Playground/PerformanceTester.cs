namespace QStore.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class PerformanceTester
    {
        private Dictionary<string, int> dictionary;

        private HashSet<string> hashSet;

        private QStringMap<int> map;

        private QStringSet set;

        private SortedDictionary<string, int> sortedDictionary;

        private SortedList<string, int> sortedList;

        private string[] words;

        public static PerformanceTester Create(string fromFile, Encoding encoding)
        {
            var tester = new PerformanceTester { words = File.ReadAllLines(fromFile, encoding) };
            // tester.InitSet();
            return tester;
        }

        public bool TestVsHashSet()
        {
            this.InitSet();
            this.InitHashSet();

            Console.WriteLine(@"Testing qset vs hashset");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            bool b = false;
            for (int i = 0; i < words.Length; i++)
            {
                b ^= this.hashSet.Contains(words[i]);
            }

            stopwatch.Stop();
            Console.WriteLine(
                @"HashSet time for {0} words: {1} ms", this.words.Length, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            for (int i = 0; i < words.Length; i++)
            {
                b ^= this.set.Contains(words[i]);
            }
            stopwatch.Stop();
            Console.WriteLine(
                @"   QSet time for {0} words: {1} ms", this.words.Length, stopwatch.ElapsedMilliseconds);
            Console.WriteLine();
            return b;
        }

        public int TestVsDictionary()
        {
            this.InitMap();
            this.InitDictionary();

            Console.WriteLine(@"Testing qmap vs dictionary");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int x = 0;
            foreach (var word in this.words)
            {
                x = this.dictionary[word];
            }
            stopwatch.Stop();
            Console.WriteLine(
                @"Dictionary time for {0} words: {1} ms", this.words.Length, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            foreach (var word in this.words)
            {
                x = this.map[word];
            }
            stopwatch.Stop();
            Console.WriteLine(
                @"       Map time for {0} words: {1} ms", this.words.Length, stopwatch.ElapsedMilliseconds);
            Console.WriteLine();
            return x;
        }

        public int TestVsSortedDictionary()
        {
            this.InitMap();
            this.InitSortedDictionary();

            Console.WriteLine(@"Testing qmap vs sorted dictionary");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int x = 0;
            foreach (var word in this.words)
            {
                x = this.sortedDictionary[word];
            }
            stopwatch.Stop();
            Console.WriteLine(
                @"Sorted dictionary time for {0} words: {1} ms", this.words.Length, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            foreach (var word in this.words)
            {
                x = this.map[word];
            }
            stopwatch.Stop();
            Console.WriteLine(
                @"              Map time for {0} words: {1} ms", this.words.Length, stopwatch.ElapsedMilliseconds);
            Console.WriteLine();
            return x;
        }

        private static long GetCurrentUsedMemory()
        {
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            return GC.GetTotalMemory(true);
        }

        private void InitDictionary()
        {
            if (this.dictionary == null)
            {
                Console.WriteLine(@"Initializing Dictionary<string, int> with {0} elements.", this.words.Length);
                var stopwatch = new Stopwatch();
                long memoryBefore = GetCurrentUsedMemory();
                stopwatch.Start();

                this.dictionary = this.words.ToDictionary(w => w, w => w.GetHashCode());

                stopwatch.Stop();
                long memoryAfter = GetCurrentUsedMemory();
                Console.WriteLine(@"Memory delta: {0:+#;-#} bytes", memoryAfter - memoryBefore);
                Console.WriteLine(@"Elapsed time:  {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }

        private void InitHashSet()
        {
            if (this.hashSet == null)
            {
                Console.WriteLine(@"Initializing HashSet<int> with {0} elements.", this.words.Length);
                var stopwatch = new Stopwatch();
                long memoryBefore = GetCurrentUsedMemory();
                stopwatch.Start();

                this.hashSet = new HashSet<string>(this.words, StringComparer.Ordinal);

                stopwatch.Stop();
                long memoryAfter = GetCurrentUsedMemory();
                Console.WriteLine(@"Memory delta: {0:+#;-#} bytes", memoryAfter - memoryBefore);
                Console.WriteLine(@"Elapsed time:  {0} ms", stopwatch.ElapsedMilliseconds);
                Console.WriteLine();
            }
        }

        private void InitMap()
        {
            if (this.map == null)
            {
                Console.WriteLine(@"Initializing QStringMap<int> with {0} elements.", this.words.Length);
                var stopwatch = new Stopwatch();
                long memoryBefore = GetCurrentUsedMemory();
                stopwatch.Start();

                this.map = QStringMap<int>.Create(this.words, Comparer<char>.Default);
                foreach (var word in this.words)
                {
                    this.map[word] = word.GetHashCode();
                }

                stopwatch.Stop();
                long memoryAfter = GetCurrentUsedMemory();
                Console.WriteLine(@"Memory delta: {0:+#;-#} bytes", memoryAfter - memoryBefore);
                Console.WriteLine(@"Elapsed time:  {0} ms", stopwatch.ElapsedMilliseconds);
                Console.WriteLine();
            }
        }

        private void InitSet()
        {
            if (this.set == null)
            {
                Console.WriteLine(@"Initializing QStringSet with {0} elements.", this.words.Length);
                var stopwatch = new Stopwatch();
                long memoryBefore = GetCurrentUsedMemory();
                stopwatch.Start();

                this.set = QStringSet.Create(this.words, Comparer<char>.Default);

                stopwatch.Stop();
                long memoryAfter = GetCurrentUsedMemory();
                Console.WriteLine(@"Memory delta: {0:+#;-#} bytes", memoryAfter - memoryBefore);
                Console.WriteLine(@"Elapsed time:  {0} ms", stopwatch.ElapsedMilliseconds);
                Console.WriteLine();
            }
        }

        private void InitSortedDictionary()
        {
            if (this.sortedDictionary == null)
            {
                Console.WriteLine(@"Initializing SortedDictionary<string, int> with {0} elements.", this.words.Length);
                var stopwatch = new Stopwatch();
                long memoryBefore = GetCurrentUsedMemory();
                stopwatch.Start();

                this.sortedDictionary = new SortedDictionary<string, int>();
                foreach (var word in this.words)
                {
                    this.sortedDictionary.Add(word, word.GetHashCode());
                }

                stopwatch.Stop();
                long memoryAfter = GetCurrentUsedMemory();
                Console.WriteLine(@"Memory delta: {0:+#;-#} bytes", memoryAfter - memoryBefore);
                Console.WriteLine(@"Elapsed time:  {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}