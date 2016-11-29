using Badger.Redis.Types;
using Xunit;

namespace Badger.Redis.Tests.Types
{
    public class RedisTypeTests
    {
        [Fact]
        public void StringPrefixTest()
        {
            Assert.Equal('+', RedisType.String.Prefix());
        }

        [Fact]
        public void ErrorPrefixTest()
        {
            Assert.Equal('-', RedisType.Error.Prefix());
        }

        [Fact]
        public void IntegerPrefixTest()
        {
            Assert.Equal(':', RedisType.Integer.Prefix());
        }

        [Fact]
        public void BulkStringPrefixTest()
        {
            Assert.Equal('$', RedisType.BulkString.Prefix());
        }

        [Fact]
        public void ArrayPrefixTest()
        {
            Assert.Equal('*', RedisType.Array.Prefix());
        }
    }
}