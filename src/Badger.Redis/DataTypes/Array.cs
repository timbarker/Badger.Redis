using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Badger.Redis.DataTypes
{
    public class Array : IDataType<IDataType[]>, IEnumerable<IDataType>, IEquatable<Array>
    {
        public static Array Null = new Array();
        public DataType DataType { get; } = DataType.Array;
        public IDataType[] Value { get; }
        public int Length => Value?.Length ?? -1;

        private Array()
        { }

        public Array(params IDataType[] value)
        {
            Value = value;
        }

        public IEnumerator<IDataType> GetEnumerator()
        {
            return (Value?.AsEnumerable() ?? Enumerable.Empty<IDataType>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            if (Value == null) return "";
            return $"[{string.Join(", ", Value.Select(v => v.ToString()))}]";
        }

        public override bool Equals(object obj)
        {
            var other = obj as Array;
            if (other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            if (Value == null) return 0;
            unchecked
            {
                return Value.Aggregate(17, (hash, element) => hash * 31 + element.GetHashCode());
            }
        }

        public bool Equals(Array other)
        {
            if (Value == null && other.Value == null) return true;
            if (Value == null || other.Value == null) return false;
            if (Value.Length != other.Value.Length) return false;

            for (int i = 0; i < Value.Length; i++)
            {
                if (!Value[i].Equals(other.Value[i]))
                    return false;
            }

            return true;
        }
    }
}
