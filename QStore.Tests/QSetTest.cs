namespace QStore.Tests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QSetTest
    {
        [TestMethod]
        public void CreeateTest()
        {
            const string Str = "str";
            ISequenceSet<char> a = null;
            bool b = a.Contains(Str);
            
        }
    }
}
