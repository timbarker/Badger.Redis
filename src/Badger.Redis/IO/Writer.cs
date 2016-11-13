﻿using Badger.Redis.DataTypes;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Array = Badger.Redis.DataTypes.Array;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.IO
{
    public class Writer : IWriter<IDataType>, IWriter<String>, IWriter<Error>, IWriter<Integer>, IWriter<BulkString>, IWriter<Array>
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

        public Task WriteAsync(IDataType value, CancellationToken cancellationToken)
        {
            switch (value.DataType)
            {
                case DataType.String:
                    return WriteAsync(value as String, cancellationToken);
                case DataType.Error:
                    return WriteAsync(value as Error, cancellationToken);
                case DataType.Integer:
                    return WriteAsync(value as Integer, cancellationToken);
                case DataType.BulkString:
                    return WriteAsync(value as BulkString, cancellationToken);
                case DataType.Array:
                    return WriteAsync(value as Array, cancellationToken);
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
            
        }

        public async Task WriteAsync(String value, CancellationToken cancellationToken)
        {
            await WriteSimpleAsync(value, cancellationToken);
        }

        public async Task WriteAsync(Error value, CancellationToken cancellationToken)
        {
            await WriteSimpleAsync(value, cancellationToken);
        }

        public async Task WriteAsync(Integer value, CancellationToken cancellationToken)
        {
            await WriteSimpleAsync(value, cancellationToken);
        }

        private async Task WriteSimpleAsync(IDataType value, CancellationToken cancellationToken)
        {
            var bytes = DefaultEncoding.GetBytes($"{value.DataType.Prefix()}{value}{NewLine}");
            await _stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        public async Task WriteAsync(BulkString value, CancellationToken cancellationToken)
        {
            var header = DefaultEncoding.GetBytes($"{DataType.BulkString.Prefix()}{value.Length}");
            await _stream.WriteAsync(header, 0, header.Length, cancellationToken);
            await _stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);

            if (value == DataTypes.BulkString.Null) return;

            await _stream.WriteAsync(value.Value, 0, value.Length, cancellationToken);
            await _stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);
        }

        public async Task WriteAsync(Array value,CancellationToken cancellationToken)
        {
            var header = DefaultEncoding.GetBytes($"{DataType.Array.Prefix()}{value.Length}");
            await _stream.WriteAsync(header, 0, header.Length, cancellationToken);
            await _stream.WriteAsync(EncodedNewLine, 0, EncodedNewLine.Length, cancellationToken);

            if (value == DataTypes.Array.Null) return;

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
