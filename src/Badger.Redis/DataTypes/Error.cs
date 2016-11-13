using System;

namespace Badger.Redis.DataTypes
{
    public class Error : IDataType<string>, IEquatable<Error>
    {
        public DataType DataType { get; } = DataType.Error;
        public string Value { get; }

        public Error(string value)
        {
            if (value == null)
                throw new ArgumentException($"{nameof(value)} can't be null", nameof(value));

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Error);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(Error other)
        {
            if (other == null) return false;
            return Value == other.Value;
        }
    }
}
