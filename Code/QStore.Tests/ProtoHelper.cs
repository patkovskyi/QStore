namespace QStore.Tests
{
    using System.IO;

    public static class ProtoHelper
    {
        public static QSet FromProto(byte[] protoBytes)
        {
            using (var ms = new MemoryStream(protoBytes))
            {
                var set = ProtoBuf.Serializer.Deserialize<QSet>(ms);
                return set;
            }
        }

        public static byte[] ToProto(QSet set)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, set);
                return ms.ToArray();
            }
        }
    }
}