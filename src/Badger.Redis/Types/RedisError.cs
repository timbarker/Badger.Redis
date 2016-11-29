using System;

namespace Badger.Redis.Types
{
    internal class RedisErorr : IRedisType<string>, IEquatable<RedisErorr>
    {
        public RedisType RedisType { get; } = RedisType.Error;
        public string Value { get; }

        public RedisErorr(string value)
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
            return Equals(obj as RedisErorr);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(RedisErorr other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public static bool operator ==(RedisErorr lhs, RedisErorr rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(RedisErorr lhs, RedisErorr rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}