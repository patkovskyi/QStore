using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using QSpell.Helpers;
using QSpell.Comparers;

namespace QSpell.Sequences
{
    [ProtoContract(IgnoreListHandling = true)]
    // see Protobuf-net inheritance hierarchy defined in ProtoBufHelper
    public class SparseSequenceDictionary<T, V> : SequenceDictionary<T, V>
    {
        public new static SparseSequenceDictionary<T, V> Create(IEnumerable<KeyValuePair<IEnumerable<T>, V>> sequences, IComparer<T> comparer, bool minimize)
        {
            return SequenceDictionary<T, V>.Create<SparseSequenceDictionary<T, V>>(sequences, comparer, minimize);
        }

        public static new SparseSequenceDictionary<T, V> Deserialize(byte[] bytes, IComparer<T> symbolComparer)
        {
            var result = ProtoBufHelper.DeserializeFromBytes<SparseSequenceDictionary<T, V>>(bytes);
            result.symbolComparer = symbolComparer;
            return result;
        }        

        [ProtoMember(7)]
        private AlignedPair<Int32, V>[] serializedValues;

        [ProtoMember(8)]
        private Int32 valuesLength;

        [ProtoBeforeSerialization]
        protected void ProtoBeforeSerialization()
        {
            serializedValues = values.Select((v, i) => new AlignedPair<Int32, V>(i, v)).Where(p => !object.Equals(p.Value, default(V))).ToArray();
            valuesLength = values.Length;
            values = null;
        }                

        [ProtoAfterSerialization]
        [ProtoAfterDeserialization]
        protected void ProtoAfterDeserialization()
        {
            values = new V[valuesLength];
            for (int i = 0; i < serializedValues.Length; i++)
            {
                values[serializedValues[i].Key] = serializedValues[i].Value;
            }
            serializedValues = null;
        }
    }
}
