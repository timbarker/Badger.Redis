using Badger.Redis.Types;
using Xunit;

namespace Badger.Redis.Tests.Types
{
    public class RedisIntegerTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var i = new RedisInteger(0);

            Assert.Equal(RedisType.Integer, i.RedisType);
        }

        [Fact]
        public void ValueIsCorrect()
        {
            var i = new RedisInteger(1234L);

            Assert.Equal(1234L, i.Value);
        }

        [Fact]
        public void ToStringCorrect()
        {
            var i = new RedisInteger(1234);

            Assert.Equal("1234", i.ToString());
        }

        [Fact]
        public void IntegersAreEqualToItself()
        {
            var i = new RedisInteger(0);

            Assert.True(i.Equals(i));
        }

        [Fact]
        public void IntegersAreNotEqualToNull()
        {
            var i = new RedisInteger(0);

            Assert.False(i.Equals(null));
        }

        [Fact]
        public void IntegersWithSameValueAreEqual()
        {
            var integer1 = new RedisInteger(1234);
            var integer2 = new RedisInteger(1234);

            Assert.True(integer1.Equals(integer2));
            Assert.True(integer2.Equals(integer1));

            Assert.True(integer1 == integer2);
            Assert.True(integer2 == integer1);
        }

        [Fact]
        public void IntegersWithDifferentValueAreNotEqual()
        {
            var integer1 = new RedisInteger(1234);
            var integer2 = new RedisInteger(4321);

            Assert.False(integer1.Equals(integer2));
            Assert.False(integer2.Equals(integer1));

            Assert.True(integer1 != integer2);
            Assert.True(integer2 != integer1);
        }

        [Fact]
        public void IntegersWithSameContentHaveSameHashCode()
        {
            var integer1 = new RedisInteger(1234);
            var integer2 = new RedisInteger(1234);

            Assert.Equal(integer1.GetHashCode(), integer2.GetHashCode());
        }
    }
}