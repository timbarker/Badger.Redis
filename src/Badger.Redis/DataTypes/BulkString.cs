using System;

namespace Badger.Redis.DataTypes
{
    public class BulkString : IDataType<string>, IEquatable<BulkString>
    {
        public static BulkString Null = new BulkString();
        public DataType DataType { get; } = DataType.BulkString;
        public string Value { get; }
        public int Length => Value?.Length ?? -1;

        private BulkString()
        {
        }

        public BulkString(string value)
        {
            if (value == null) return;

            Value = value;
        }

        public static implicit operator string(BulkString s) => s.Value;
        public static implicit operator BulkString(string s) => s == null ? Null : new BulkString(s);

        public override string ToString()
        {
            return Value == null ? "" : Value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BulkString;
            if (other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public bool Equals(BulkString other)
        {
            return Value == other.Value;
        }
    }
}
