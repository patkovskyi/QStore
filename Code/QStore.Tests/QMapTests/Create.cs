namespace QStore.Tests.QMapTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class Create
    {
        [Test]
        public void NotImplemented()
        {
            Assert.Throws<NotImplementedException>(
                () => QMap<int>.Create(new[] { new KeyValuePair<string, int>("a", 0) }));
        }
    }
}