using System.Text;

namespace Badger.Redis.Types
{
    internal class RedisKey : RedisBulkString
    {
        public RedisKey(params object[] parts)
            : this(string.Join(":", parts))
        { }

        public RedisKey(char seperator, params object[] parts)
            : this(string.Join(seperator.ToString(), parts))
        { }

        public RedisKey(string key)
            : base(Encoding.UTF8.GetBytes(key))
        { }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Value);
        }
    }
}