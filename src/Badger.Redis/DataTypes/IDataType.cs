namespace Badger.Redis.DataTypes
{
    internal interface IDataType
    {
        DataType DataType { get; }
    }

    internal interface IDataType<T> : IDataType
    {
        T Value { get; }
    }
}