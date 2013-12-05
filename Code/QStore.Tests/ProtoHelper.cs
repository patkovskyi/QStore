namespace QStore.Tests
{
    using System.IO;

    public static class ProtoHelper
    {      
        public static SimpleSet FromProto(byte[] protoBytes)
        {
            using (var ms = new MemoryStream(protoBytes))
            {
                var set = ProtoBuf.Serializer.Deserialize<SimpleSet>(ms);                
                return set;
            }
        }

        public static byte[] ToProto(SimpleSet set)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, set);
                return ms.ToArray();
            }
        }
    }
}