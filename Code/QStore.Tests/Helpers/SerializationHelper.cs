namespace QStore.Tests.Helpers
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    using ProtoBuf;

    using QStore.Strings.Interfaces;

    public static class SerializationHelper
    {
        public static T BinaryDeserialize<T>(this byte[] data) where T : IStringSet
        {
            using (var ms = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }

        public static T BinaryLoop<T>(T map) where T : IStringSet
        {
            var comparer = map.Comparer;
            var bytes = map.BinarySerialize();
            map = bytes.BinaryDeserialize<T>();
            map.SetComparer(comparer);
            return map;
        }

        public static byte[] BinarySerialize<T>(this T obj) where T : IStringSet
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T BsonDeserialize<T>(this byte[] data) where T : IStringSet
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new BsonReader(ms))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        public static T BsonLoop<T>(T map) where T : IStringSet
        {
            var comparer = map.Comparer;
            var bytes = map.BsonSerialize();
            map = bytes.BsonDeserialize<T>();
            map.SetComparer(comparer);
            return map;
        }

        public static byte[] BsonSerialize<T>(this T obj) where T : IStringSet
        {
            using (var ms = new MemoryStream())
            using (var writer = new BsonWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
                return ms.ToArray();
            }
        }

        public static T DataContractDeserialize<T>(this byte[] data) where T : IStringSet
        {
            using (var stream = new MemoryStream(data))
            {
                var deserializer = new DataContractSerializer(typeof(T));
                return (T)deserializer.ReadObject(stream);
            }
        }

        public static T DataContractLoop<T>(T map) where T : IStringSet
        {
            var comparer = map.Comparer;
            var bytes = map.DataContractSerialize();
            map = bytes.DataContractDeserialize<T>();
            map.SetComparer(comparer);
            return map;
        }

        public static byte[] DataContractSerialize<T>(this T obj) where T : IStringSet
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ProtoDeserialize<T>(this byte[] data) where T : IStringSet
        {
            using (var ms = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static T ProtoLoop<T>(T map) where T : IStringSet
        {
            var comparer = map.Comparer;
            var bytes = map.ProtoSerialize();
            map = bytes.ProtoDeserialize<T>();
            map.SetComparer(comparer);
            return map;
        }

        public static byte[] ProtoSerialize<T>(this T obj) where T : IStringSet
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}