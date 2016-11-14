namespace Badger.Redis.DataTypes
{
    public interface IDataType
    {
        DataType DataType { get; }
    }

    public interface IDataType<T> : IDataType
    {
        T Value { get; }
    }
}