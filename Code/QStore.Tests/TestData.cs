namespace QStore.Tests
{
    using System.Text;

    public class TestData
    {
        public const string ZaliznyakSolutionPath = @"..\TestData\Zaliznyak-1251.txt";

        public const string ZaliznyakDeployedPath = @"Zaliznyak-1251.txt";

        public const string BaseformsSolutionPath = @"..\TestData\Zaliznyak-1251.txt";

        public const string BaseformsDeployedPath = @"Zaliznyak-baseforms-1251.txt";

        public static readonly Encoding Encoding = Encoding.GetEncoding(1251);
    }
}