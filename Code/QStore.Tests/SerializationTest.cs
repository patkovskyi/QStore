namespace QStore.Tests
{
    using System.IO;

    using NUnit.Framework;
    
    [TestFixture]
    public class SerializationTest
    {
        [Test]
        public void Test()
        {
            using (var ms = new MemoryStream())
            {
                var ss = new SimpleSet();
                
                // ss.SetFields();
                byte[] bytes = ProtoHelper.ToProto(ss);
                SimpleSet roundTrip = ProtoHelper.FromProto(bytes);
            }
        }
    }
}