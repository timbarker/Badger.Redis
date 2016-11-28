using System;

namespace Badger.Redis.DataTypes
{
    internal class Integer : IDataType<long>, IEquatable<Integer>
    {
        public DataType DataType { get; } = DataType.Integer;
        public long Value { get; }

        public Integer(long value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Integer);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(Integer other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public static bool operator ==(Integer lhs, Integer rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Integer lhs, Integer rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}