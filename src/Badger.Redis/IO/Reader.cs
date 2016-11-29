using Badger.Redis.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal class Reader : IReader
    {
        private readonly Stream _stream;
        private readonly List<byte> _readBuffer;
        private readonly byte[] _lineBuffer;

        private static readonly Encoding DefaultEncoding = Encoding.ASCII;

        public Reader(Stream stream)
        {
            _stream = stream;
            _readBuffer = new List<byte>();
            _lineBuffer = new byte[1024];
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public async Task<IRedisType> ReadAsync(CancellationToken cancellationToken)
        {
            var line = await ReadLineAsync(cancellationToken);
            var prefix = line[0];
            var value = line.Substring(1);

            switch (prefix)
            {
                case RedisTypePrefix.String:
                    return new RedisString(value);

                case RedisTypePrefix.Error:
                    return new RedisErorr(value);

                case RedisTypePrefix.Integer:
                    return ReadInteger(value);

                case RedisTypePrefix.BulkString:
                    return await ReadBulkStringAsync(value, cancellationToken);

                case RedisTypePrefix.Array:
                    return await ReadArrayAsync(value, cancellationToken);

                default:
                    throw new IOException($"Invalid prefix '{prefix}'");
            }
        }

        private IRedisType ReadInteger(string value)
        {
            long integer;
            if (!long.TryParse(value, out integer))
                throw new IOException($"Invalid Integer value '{value}'");

            return new RedisInteger(integer);
        }

        private async Task<IRedisType> ReadBulkStringAsync(string value, CancellationToken cancellationToken)
        {
            int length;
            if (!int.TryParse(value, out length) || length < -1)
                throw new IOException($"Invalid BulkString length '{value}'");

            if (length == -1) return RedisBulkString.Null;

            var bytes = await ReadBytesAsync(length, cancellationToken);
            await ReadLineAsync(cancellationToken);
            return new RedisBulkString(bytes);
        }

        private async Task<IRedisType> ReadArrayAsync(string value, CancellationToken cancellationToken)
        {
            int length;
            if (!int.TryParse(value, out length) || length < -1)
                throw new IOException($"Invalid Array length '{value}'");

            if (length == -1) return RedisArray.Null;

            var items = new IRedisType[length];
            for (int i = 0; i < length; i++)
            {
                items[i] = await ReadAsync(cancellationToken);
            }

            return new RedisArray(items);
        }

        private async Task<string> ReadLineAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                for (int i = 1; i < _readBuffer.Count; i++)
                {
                    if (_readBuffer[i - 1] == '\r' && _readBuffer[i] == '\n')
                    {
                        var line = DefaultEncoding.GetString(_readBuffer.ToArray(), 0, i - 1);
                        _readBuffer.RemoveRange(0, i + 1);
                        return line;
                    }
                }

                var bytesRead = await _stream.ReadAsync(_lineBuffer, 0, _lineBuffer.Length, cancellationToken);

                _readBuffer.AddRange(new ArraySegment<byte>(_lineBuffer, 0, bytesRead));
            }
        }

        private async Task<byte[]> ReadBytesAsync(int count, CancellationToken cancellationToken)
        {
            var readBuffer = new byte[count];
            var bytesRead = Math.Min(count, _readBuffer.Count);
            _readBuffer.CopyTo(0, readBuffer, 0, bytesRead);
            _readBuffer.RemoveRange(0, bytesRead);

            while (bytesRead < count)
            {
                bytesRead += await _stream.ReadAsync(readBuffer, bytesRead, count - bytesRead, cancellationToken);
            }

            return readBuffer;
        }
    }
}