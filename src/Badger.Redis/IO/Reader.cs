using Badger.Redis.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Array = Badger.Redis.DataTypes.Array;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.IO
{
    internal class Reader : IReader
    {
        private Stream _stream;
        private List<byte> _readBuffer;
        private byte[] _lineBuffer;

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

        public async Task<IDataType> ReadAsync(CancellationToken cancellationToken)
        {
            var line = await ReadLineAsync(cancellationToken);
            var prefix = line[0];
            var value = line.Substring(1);

            switch (prefix)
            {
                case DataTypePrefix.String:
                    return new String(value);

                case DataTypePrefix.Error:
                    return new Error(value);

                case DataTypePrefix.Integer:
                    return ReadInteger(value);

                case DataTypePrefix.BulkString:
                    return await ReadBulkStringAsync(value, cancellationToken);

                case DataTypePrefix.Array:
                    return await ReadArrayAsync(value, cancellationToken);

                default:
                    throw new IOException($"Invalid prefix '{prefix}'");
            }
        }

        private IDataType ReadInteger(string value)
        {
            long integer;
            if (!long.TryParse(value, out integer))
                throw new IOException($"Invalid Integer value '{value}'");

            return new Integer(integer);
        }

        private async Task<IDataType> ReadBulkStringAsync(string value, CancellationToken cancellationToken)
        {
            int length;
            if (!int.TryParse(value, out length) || length < -1)
                throw new IOException($"Invalid BulkString length '{value}'");

            if (length == -1) return BulkString.Null;

            var bytes = await ReadBytesAsync(length, cancellationToken);
            await ReadLineAsync(cancellationToken);
            return new BulkString(bytes);
        }

        private async Task<IDataType> ReadArrayAsync(string value, CancellationToken cancellationToken)
        {
            int length;
            if (!int.TryParse(value, out length) || length < -1)
                throw new IOException($"Invalid Array length '{value}'");

            if (length == -1) return Array.Null;

            var items = new IDataType[length];
            for (int i = 0; i < length; i++)
            {
                items[i] = await ReadAsync(cancellationToken);
            }

            return new Array(items);
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
            int bytesRead = Math.Min(count, _readBuffer.Count);
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