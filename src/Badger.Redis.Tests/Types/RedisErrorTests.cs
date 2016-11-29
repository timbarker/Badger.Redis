using Badger.Redis.Types;
using System;
using Xunit;

namespace Badger.Redis.Tests.Types
{
    public class RedisErrorTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var e = new RedisErorr("");

            Assert.Equal(RedisType.Error, e.DataType);
        }

        [Fact]
        public void ValueIsCorrect()
        {
            var e = new RedisErorr("test");

            Assert.Equal("test", e.Value);
        }

        [Fact]
        public void ToStringCorrect()
        {
            var e = new RedisErorr("error message");

            Assert.Equal("error message", e.ToString());
        }

        [Fact]
        public void ConstructingWithNullStringNotAllowed()
        {
            var ex = Assert.Throws<ArgumentException>(() => new RedisErorr(null));

            Assert.StartsWith("value can't be null", ex.Message);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void ErrorsAreEqualToItself()
        {
            var e = new RedisErorr("test");

            Assert.True(e.Equals(e));
        }

        [Fact]
        public void ErrorsAreNotEqualToNull()
        {
            var e = new RedisErorr("test");

            Assert.False(e.Equals(null));
        }

        [Fact]
        public void ErrorsWithSameContentAreEqual()
        {
            var error1 = new RedisErorr("test");
            var error2 = new RedisErorr("test");

            Assert.True(error1.Equals(error2));
            Assert.True(error2.Equals(error1));

            Assert.True(error1 == error2);
            Assert.True(error2 == error1);
        }

        [Fact]
        public void ErrorsWithDifferentContentAreNotEqual()
        {
            var error1 = new RedisErorr("test1");
            var error2 = new RedisErorr("test2");

            Assert.False(error1.Equals(error2));
            Assert.False(error2.Equals(error1));

            Assert.True(error1 != error2);
            Assert.True(error2 != error1);
        }

        [Fact]
        public void ErrorsWithSameContentHaveSameHashCode()
        {
            var error1 = new RedisErorr("test");
            var error2 = new RedisErorr("test");

            Assert.Equal(error1.GetHashCode(), error2.GetHashCode());
        }
    }
}