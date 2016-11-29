using System;

namespace Badger.Redis.Types
{
    internal static class RedisTypeExtensions
    {
        public static char Prefix(this RedisType dataType)
        {
            switch (dataType)
            {
                case RedisType.String:
                    return RedisTypePrefix.String;

                case RedisType.Error:
                    return RedisTypePrefix.Error;

                case RedisType.Integer:
                    return RedisTypePrefix.Integer;

                case RedisType.BulkString:
                    return RedisTypePrefix.BulkString;

                case RedisType.Array:
                    return RedisTypePrefix.Array;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType));
            }
        }
    }
}