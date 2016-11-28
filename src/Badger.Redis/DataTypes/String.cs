using System;

namespace Badger.Redis.DataTypes
{
    internal class String : IDataType<string>, IEquatable<String>
    {
        public DataType DataType { get; } = DataType.String;
        public string Value { get; }

        public String(string value)
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
            return Equals(obj as String);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(String other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public static bool operator ==(String lhs, String rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(String lhs, String rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}