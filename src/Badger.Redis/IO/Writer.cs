using Badger.Redis.Types;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal class Writer : IWriter
    {
        private const string NewLine = "\r\n";
        private static readonly Encoding DefaultEncoding;
        private static readonly byte[] EncodedNewLine;
        private readonly Stream _stream;

        static Writer()
        {
            DefaultEncoding = Encoding.ASCII;
            EncodedNewLine = DefaultEncoding.GetBytes(NewLine);
        }

        public Writer(Stream stream)
        {
            _stream = stream;
        }

        public Task WriteAsync(IRedisType value, CancellationToken cancellationToken)
        {
            switch (value.DataType)
            {
                case RedisType.String:
                    return WriteAsync(value as RedisString, cancellationToken);

                case RedisType.Error:
                    return WriteAsync(value as RedisErorr, cancellationToken);

                case RedisType.Integer:
                    return WriteAsync(value as RedisInteger, cancellationToken);

                case RedisType.BulkString:
                    return WriteAsync(value as RedisBulkString, cancellationToken);

                case RedisType.Array:
                    return WriteAsync(value as RedisArray, cancellationToken);

                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        private async Task WriteAsync(RedisString value, CancellationToken cancellationToken)
        {
            await WriteSimpleAsync(value, cancellationToken);
        }

        private async Task WriteAsync(RedisErorr value, CancellationToken cancellationToken)
        {
            await WriteSimpleAsync(value, cancellationToken);
        }

        private async Task WriteAsync(RedisInteger value, CancellationToken cancellationToken)
        {
            await WriteSimpleAsync(value, cancellationToken);
        }

        private async Task WriteSimpleAsync(IRedisType value, CancellationToken cancellationToken)
        {
            var bytes = DefaultEncoding.GetBytes($"{value.DataType.Prefix()}{value}{NewLine}");
            await _stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        private async Task WriteAsync(RedisBulkString value, CancellationToken cancellationToken)
        {
            var header = DefaultEncoding.GetBytes($"{RedisType.BulkString.Prefix()}{value.Length}");
            await _stream.WriteAsync(header, 0, header.Length, cancellationToken);
            await _stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);

            if (value == RedisBulkString.Null) return;

            await _stream.WriteAsync(value.Value, 0, value.Length, cancellationToken);
            await _stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);
        }

        private async Task WriteAsync(RedisArray value, CancellationToken cancellationToken)
        {
            var header = DefaultEncoding.GetBytes($"{RedisType.Array.Prefix()}{value.Length}");
            await _stream.WriteAsync(header, 0, header.Length, cancellationToken);
            await _stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);

            if (value == RedisArray.Null) return;

            foreach (var element in value.Value)
            {
                await WriteAsync(element, cancellationToken);
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}