using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;
using QSpell.Sequences;

namespace QSpell.Helpers
{
    internal static class ProtoBufHelper
    {
        static ProtoBufHelper()
        {
            RuntimeTypeModel.Default[typeof(SequenceSet<char>)]
                .AddSubType(1000, typeof(SequenceDictionary<char, byte>))
                .AddSubType(1001, typeof(SequenceDictionary<char, double>))
                .AddSubType(1004, typeof(SequenceDictionary<char, float>));

            RuntimeTypeModel.Default[typeof(SequenceDictionary<char, byte>)]
                .AddSubType(1002, typeof(SparseSequenceDictionary<char, byte>));
            
            RuntimeTypeModel.Default[typeof(SequenceDictionary<char, double>)]
                .AddSubType(1003, typeof(SparseSequenceDictionary<char, double>));

            RuntimeTypeModel.Default[typeof(SequenceDictionary<char, float>)]
                .AddSubType(1005, typeof(SparseSequenceDictionary<char, float>));
        }

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
