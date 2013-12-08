namespace QStore.Tests
{
    using System;
    using System.Runtime.InteropServices;

    using NUnit.Framework;

    [TestFixture]
    public class MemCmpTest
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(QTransition[] b1, QTransition[] b2, long count);

        [Test]
        public void TryCmp()
        {
            var tr1 = new[]
            { new QTransition('a', 1, false), new QTransition('b', 2, true), new QTransition('z', 7, false) };

            var tr2 = new[]
            { new QTransition('a', 1, false), new QTransition('b', 2, true), new QTransition('z', 7, false) };
            
            int size = Marshal.SizeOf(typeof(QTransition));
            Assert.AreEqual(0, memcmp(tr1, tr2, tr1.Length * size));
        }
    }
}