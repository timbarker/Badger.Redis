namespace Badger.Redis.Types
{
    internal interface IRedisType
    {
        RedisType RedisType { get; }
    }

    internal interface IRedisType<T> : IRedisType
    {
        T Value { get; }
    }
}