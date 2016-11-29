using System;

namespace Badger.Redis.Types
{
    internal class RedisString : IRedisType<string>, IEquatable<RedisString>
    {
        public RedisType DataType { get; } = RedisType.String;
        public string Value { get; }

        public RedisString(string value)
        {
            if (value == null)
                throw new ArgumentException($"{nameof(value)} can't be null", nameof(value));

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RedisString);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(RedisString other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public static bool operator ==(RedisString lhs, RedisString rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(RedisString lhs, RedisString rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}