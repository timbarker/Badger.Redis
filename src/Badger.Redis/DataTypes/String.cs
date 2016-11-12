﻿using System;

namespace Badger.Redis.DataTypes
{
    public class String : IDataType<string>, IEquatable<String>
    {
        public DataType DataType { get; } = DataType.String;
        public string Value { get; }

        public String(string value)
        {
            if (value == null)
                throw new ArgumentException($"{nameof(value)} can't be null", nameof(value));

            Value = value;
        }

        public static implicit operator string(String s) => s.Value;
        public static implicit operator String(string s) => new String(s);

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as String;
            if (other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(String other)
        {
            return Value == other.Value;
        }
    }
}
