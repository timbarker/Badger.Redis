using System;

namespace Badger.Redis.Types
{
    internal class RedisInteger : IRedisType<long>, IEquatable<RedisInteger>
    {
        public RedisType DataType { get; } = RedisType.Integer;
        public long Value { get; }

        public RedisInteger(long value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RedisInteger);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(RedisInteger other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public static bool operator ==(RedisInteger lhs, RedisInteger rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(RedisInteger lhs, RedisInteger rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}