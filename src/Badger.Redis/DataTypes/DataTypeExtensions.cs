using System;

namespace Badger.Redis.DataTypes
{
    internal static class DataTypeExtensions
    {
        public static char Prefix(this DataType dataType)
        {
            switch (dataType)
            {
                case DataType.String:
                    return DataTypePrefix.String;

                case DataType.Error:
                    return DataTypePrefix.Error;

                case DataType.Integer:
                    return DataTypePrefix.Integer;

                case DataType.BulkString:
                    return DataTypePrefix.BulkString;

                case DataType.Array:
                    return DataTypePrefix.Array;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType));
            }
        }
    }
}