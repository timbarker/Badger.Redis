namespace Badger.Redis.Types
{
    internal static class RedisTypePrefix
    {
        public const char String = '+';
        public const char Error = '-';
        public const char Integer = ':';
        public const char BulkString = '$';
        public const char Array = '*';
    }
}