namespace Badger.Redis.DataTypes
{
    public sealed class DataType
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
    }
}
