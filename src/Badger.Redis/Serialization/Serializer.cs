using Badger.Redis.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using Array = Badger.Redis.DataTypes.Array;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.Serialization
{
    public static class Serializer
    {
        public static ISerializer<String> String { get; } = new BasicSerializer<String>();
        public static ISerializer<Error> Error { get; } = new BasicSerializer<Error>();
        public static ISerializer<Integer> Integer { get; } = new BasicSerializer<Integer>();
        public static ISerializer<BulkString> BulkString { get; } = new BulkStringSerializer();
        public static ISerializer<Array> Array { get; } = new ArraySerializer();

        private class BasicSerializer<T> : ISerializer<T> where T : IDataType
        {
            public string Serialize(T value)
            {
                return $"{value.DataType.Prefix}{value}\r\n";
            }
        }

        private class BulkStringSerializer : ISerializer<BulkString>
        {
            public string Serialize(BulkString value)
            {
                var valueString = value == DataTypes.BulkString.Null ? "" : value + "\r\n";
                return $"{DataType.BulkString.Prefix}{value.Length}\r\n{valueString}";
            }
        }

        private class ArraySerializer : ISerializer<Array>
        {
            private static readonly IDictionary<DataType, Func<IDataType, string>> SerializeType
                = new Dictionary<DataType, Func<IDataType, string>>
            {
                { DataType.String, value => String.Serialize(value as String) },
                { DataType.Integer, value => Integer.Serialize(value as Integer) },
                { DataType.Error, value => Error.Serialize(value as Error) },
                { DataType.BulkString, value => BulkString.Serialize(value as BulkString) },
                { DataType.Array, value => Array.Serialize(value as Array) },
            };

            public string Serialize(Array value)
            {
                var serializedValues = value.Select(v => SerializeType[v.DataType](v));
                return $"{DataType.Array.Prefix}{value.Length}\r\n{string.Join("", serializedValues)}";
            }
        }
    }
}
