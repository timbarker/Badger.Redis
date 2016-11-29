using System;
using System.Linq;
using System.Text;

namespace Badger.Redis.Types
{
    internal class RedisBulkString : IRedisType<byte[]>, IEquatable<RedisBulkString>
    {
        private const int MaxSize = 512 * 1024 * 1024;

        public static RedisBulkString Null = new RedisBulkString();
        public RedisType DataType { get; } = RedisType.BulkString;
        public byte[] Value { get; }
        public int Length => Value?.Length ?? -1;

        private RedisBulkString()
        {
        }

        public RedisBulkString(byte[] value)
        {
            if (value == null) return;

            if (value.Length > MaxSize)
                throw new ArgumentException($"{nameof(value)} is larger than {MaxSize} bytes", nameof(value));

            Value = value;
        }

        public static RedisBulkString FromString(string value, Encoding encoding = null)
        {
            return new RedisBulkString((encoding ?? Encoding.UTF8).GetBytes(value));
        }

        public override string ToString()
        {
            return Value == null ? "" : "0x" + string.Join("", Value.Select(e => e.ToString("x")));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RedisBulkString);
        }

        public override int GetHashCode()
        {
            if (Value == null) return 0;
            unchecked
            {
                return Value.Aggregate(17, (hash, element) => hash * 31 + element.GetHashCode());
            }
        }

        public bool Equals(RedisBulkString other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Value == null && other.Value == null) return true;
            if (Value == null || other.Value == null) return false;
            if (Value.Length != other.Value.Length) return false;
            return Value.SequenceEqual(other.Value);
        }

        public static bool operator ==(RedisBulkString lhs, RedisBulkString rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(RedisBulkString lhs, RedisBulkString rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}