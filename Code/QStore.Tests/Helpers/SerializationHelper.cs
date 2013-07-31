namespace QStore.Tests.Helpers
{
    using System;
    using System.Diagnostics;
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
                var watch = new Stopwatch();
                watch.Start();
                var obj = (T)formatter.Deserialize(ms);
                Console.WriteLine(@"Binary deserialize: {0} ms", watch.ElapsedMilliseconds);
                return obj;
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
                var watch = new Stopwatch();
                watch.Start();
                formatter.Serialize(ms, obj);
                Console.WriteLine(@"Binary serialize: {0} ms", watch.ElapsedMilliseconds);
                var bytes = ms.ToArray();
                Console.WriteLine(@"Binary footprint: {0} bytes", bytes.Length);
                return bytes;
            }
        }

        public static T BsonDeserialize<T>(this byte[] data) where T : IStringSet
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new BsonReader(ms))
            {
                var serializer = new JsonSerializer();
                var watch = new Stopwatch();
                watch.Start();
                var obj = serializer.Deserialize<T>(reader);
                Console.WriteLine(@"Bson deserialize: {0} ms", watch.ElapsedMilliseconds);
                return obj;
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
                var watch = new Stopwatch();
                watch.Start();
                serializer.Serialize(writer, obj);
                Console.WriteLine(@"Bson serialize: {0} ms", watch.ElapsedMilliseconds);
                var bytes = ms.ToArray();
                Console.WriteLine(@"Bson footprint: {0} bytes", bytes.Length);
                return bytes;
            }
        }

        public static T DataContractDeserialize<T>(this byte[] data) where T : IStringSet
        {
            using (var stream = new MemoryStream(data))
            {
                var deserializer = new DataContractSerializer(typeof(T));
                var watch = new Stopwatch();
                watch.Start();
                var obj = (T)deserializer.ReadObject(stream);
                Console.WriteLine(@"DataContract deserialize: {0}ms", watch.ElapsedMilliseconds);
                return obj;
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
                var watch = new Stopwatch();
                watch.Start();
                serializer.WriteObject(ms, obj);
                Console.WriteLine(@"DataContract serialize: {0} ms", watch.ElapsedMilliseconds);
                var bytes = ms.ToArray();
                Console.WriteLine(@"DataContract footprint: {0} bytes", bytes.Length);
                return bytes;
            }
        }

        public static T ProtoDeserialize<T>(this byte[] data) where T : IStringSet
        {
            using (var ms = new MemoryStream(data))
            {
                var watch = new Stopwatch();
                watch.Start();
                var obj = Serializer.Deserialize<T>(ms);
                Console.WriteLine(@"Proto deserialize: {0} ms", watch.ElapsedMilliseconds);
                return obj;
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
                var watch = new Stopwatch();
                watch.Start();
                Serializer.Serialize(ms, obj);
                Console.WriteLine(@"Proto serialize: {0} ms", watch.ElapsedMilliseconds);
                var bytes = ms.ToArray();
                Console.WriteLine(@"Proto footprint: {0} bytes", bytes.Length);
                return bytes;
            }
        }
    }
}