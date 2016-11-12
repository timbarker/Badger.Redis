using System;

namespace Badger.Redis.DataTypes
{
    public class Integer : IDataType<long>, IEquatable<Integer>
    {
        public DataType DataType { get; } = DataType.Integer;
        public long Value { get; }

        public Integer(long value)
        {
            Value = value;
        }

        public static implicit operator long(Integer i) => i.Value;
        public static implicit operator Integer(long i) => new Integer(i);

        public override string ToString()
        {
            return Value.ToString();
        }
        public override bool Equals(object obj)
        {
            var other = obj as Integer;
            if (other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(Integer other)
        {
            return Value == other.Value;
        }
    }
}
