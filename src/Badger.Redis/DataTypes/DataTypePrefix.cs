namespace Badger.Redis.DataTypes
{
    public static class DataTypePrefix
    {
        public const char String = '+';
        public const char Error = '-';
        public const char Integer = ':';
        public const char BulkString = '$';
        public const char Array = '*';
    }
}