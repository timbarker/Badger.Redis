using Badger.Redis.DataTypes;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Array = Badger.Redis.DataTypes.Array;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.IO
{
    public static class Writer
    {
        public static IWriter<String> String { get; } = new BasicWriter<String>();
        public static IWriter<Error> Error { get; } = new BasicWriter<Error>();
        public static IWriter<Integer> Integer { get; } = new BasicWriter<Integer>();
        public static IWriter<BulkString> BulkString { get; } = new BulkStringWriter();
        public static IWriter<Array> Array { get; } = new ArrayWriter();

        private const string NewLine = "\r\n";
        private static readonly Encoding DefaultEncoding;
        private static readonly byte[] EncodedNewLine;

        static Writer() 
        {
            DefaultEncoding = Encoding.ASCII;
            EncodedNewLine = DefaultEncoding.GetBytes(NewLine);
        }

        private class BasicWriter<T> : IWriter<T> where T : IDataType
        {
            public async Task WriteAsync(T value, Stream stream, CancellationToken cancellationToken)
            {
                var bytes = DefaultEncoding.GetBytes($"{value.DataType.Prefix}{value}{NewLine}");
                await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            }
        }

        private class BulkStringWriter : IWriter<BulkString>
        {
            public async Task WriteAsync(BulkString value, Stream stream, CancellationToken cancellationToken)
            {
                var header = DefaultEncoding.GetBytes($"{DataType.BulkString.Prefix}{value.Length}");
                await stream.WriteAsync(header, 0, header.Length, cancellationToken);
                await stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);

                if (value == DataTypes.BulkString.Null) return;

                await stream.WriteAsync(value.Value, 0, value.Length, cancellationToken);
                await stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);
            }
        }

        private class ArrayWriter : IWriter<Array>
        {
            private delegate Task WriteAsyncDelegate(IDataType value, Stream stream, CancellationToken cancellationToken);

            private static readonly IDictionary<DataType, WriteAsyncDelegate> Writers
                = new Dictionary<DataType, WriteAsyncDelegate>
            {
                { DataType.String, (value, stream, token) => String.WriteAsync(value as String, stream, token) },
                { DataType.Integer, (value, stream, token) => Integer.WriteAsync(value as Integer, stream, token) },
                { DataType.Error, (value, stream, token) => Error.WriteAsync(value as Error, stream, token) },
                { DataType.BulkString, (value, stream, token) => BulkString.WriteAsync(value as BulkString, stream, token) },
                { DataType.Array, (value, stream, token) => Array.WriteAsync(value as Array, stream, token) },
            };

            private Task WriteAsync(IDataType value, Stream stream, CancellationToken cancellationToken)
            {
                return Writers[value.DataType](value, stream, cancellationToken);
            }

            public async Task WriteAsync(Array value, Stream stream, CancellationToken cancellationToken)
            {
                var header = DefaultEncoding.GetBytes($"{DataType.Array.Prefix}{value.Length}");
                await stream.WriteAsync(header, 0, header.Length, cancellationToken);
                await stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);

                if (value == DataTypes.Array.Null) return;

                foreach (var element in value.Value)
                {
                    await WriteAsync(element, stream, cancellationToken);
                }
            }
        }
    }
}
