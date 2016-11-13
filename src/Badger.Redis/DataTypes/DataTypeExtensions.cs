using System;

namespace Badger.Redis.DataTypes
{
    public static class DataTypeExtensions
    {
        public static char Prefix(this DataType dataType)
        {
            switch (dataType)
            {
                case DataType.String:
                    return '+';
                case DataType.Error:
                    return '-';
                case DataType.Integer:
                    return ':';
                case DataType.BulkString:
                    return '$';
                case DataType.Array:
                    return '*';
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType));
            }
        }
    }
}
