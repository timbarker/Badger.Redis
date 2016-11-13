using System;

namespace Badger.Redis.DataTypes
{
    public sealed class DataType : IEquatable<DataType>
    {
        private DataType(char prefix)
        {
            Prefix = prefix;
        }

        public char Prefix { get; }
        public static DataType String { get; } = new DataType('+');
        public static DataType Error { get; } = new DataType('-');
        public static DataType Integer { get; } = new DataType(':');
        public static DataType BulkString { get; } = new DataType('$');
        public static DataType Array { get; } = new DataType('*');

        public override int GetHashCode()
        {
            return Prefix.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as DataType);
        }

        public bool Equals(DataType other)
        {
            if (other == null) return false;
            return Prefix == other.Prefix;
        }
    }
}
