namespace Badger.Redis.Types
{
    internal interface IRedisType
    {
        RedisType DataType { get; }
    }

    internal interface IRedisType<T> : IRedisType
    {
        T Value { get; }
    }
}