using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Badger.Redis.Types
{
    internal class RedisArray : IRedisType<IRedisType[]>, IEnumerable<IRedisType>, IEquatable<RedisArray>
    {
        public static RedisArray Null = new RedisArray();
        public RedisType RedisType { get; } = RedisType.Array;
        public IRedisType[] Value { get; }
        public int Length => Value?.Length ?? -1;

        private RedisArray()
        { }

        public RedisArray(params IRedisType[] value)
        {
            Value = value;
        }

        public IEnumerator<IRedisType> GetEnumerator()
        {
            return (Value?.AsEnumerable() ?? Enumerable.Empty<IRedisType>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (Value == null) return "";
            return $"[{string.Join(", ", Value.Select(v => v.ToString()))}]";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RedisArray);
        }

        public override int GetHashCode()
        {
            if (Value == null) return 0;
            unchecked
            {
                return Value.Aggregate(17, (hash, element) => hash * 31 + element.GetHashCode());
            }
        }

        public bool Equals(RedisArray other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Value == null && other.Value == null) return true;
            if (Value == null || other.Value == null) return false;
            if (Value.Length != other.Value.Length) return false;
            return Value.SequenceEqual(other.Value);
        }

        public static bool operator ==(RedisArray lhs, RedisArray rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(RedisArray lhs, RedisArray rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}