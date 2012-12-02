namespace QStore.Tests
{
    using System.IO;
    using System.Text;

    public static class TestData
    {
        public const string TestDataDir = @"..\..\..\NotCode\TestData";

        public static readonly string[] Zaliznyak = File.ReadAllLines(
            Path.Combine(TestDataDir, @"Zaliznyak-1251.txt"), Encoding.GetEncoding(1251));

        public static readonly string[] ZaliznyakBaseforms =
            File.ReadAllLines(Path.Combine(TestDataDir, @"Zaliznyak-baseforms-1251.txt"), Encoding.GetEncoding(1251));
    }
}