using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;

namespace QSpell.Helpers
{
    internal static class ProtoBufHelper
    {
        public static byte[] SerializeAsBytes<T>(T obj)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, obj);
            return stream.ToArray();
        }

        public static T DeserializeFromBytes<T>(byte[] buffer)
        {
            T res = Serializer.Deserialize<T>(new MemoryStream(buffer));
            return res;
        }
    }
}
