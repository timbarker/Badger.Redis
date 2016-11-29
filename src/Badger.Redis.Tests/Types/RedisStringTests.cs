using Badger.Redis.Types;
using System;
using Xunit;

namespace Badger.Redis.Tests.Types
{
    public class RedisStringTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var s = new RedisString("");

            Assert.Equal(RedisType.String, s.RedisType);
        }

        [Fact]
        public void ValueIsCorrect()
        {
            var s = new RedisString("test");

            Assert.Equal("test", s.Value);
        }

        [Fact]
        public void ToStringCorrect()
        {
            var s = new RedisString("test");

            Assert.Equal("test", s.ToString());
        }

        [Fact]
        public void ConstructingWithNullStringNotAllowed()
        {
            var ex = Assert.Throws<ArgumentException>(() => new RedisString(null));

            Assert.StartsWith("value can't be null", ex.Message);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void StringsAreEqualToItself()
        {
            var s = new RedisString("test");

            Assert.True(s.Equals(s));
        }

        [Fact]
        public void StringsAreNotEqualToNull()
        {
            var s = new RedisString("test");

            Assert.False(s.Equals(null));
        }

        [Fact]
        public void StringsWithSameContentAreEqual()
        {
            var string1 = new RedisString("test");
            var string2 = new RedisString("test");

            Assert.True(string1.Equals(string2));
            Assert.True(string2.Equals(string1));

            Assert.True(string1 == string2);
            Assert.True(string2 == string1);
        }

        [Fact]
        public void StringsWithDifferentContentAreNotEqual()
        {
            var string1 = new RedisString("test1");
            var string2 = new RedisString("test2");

            Assert.False(string1.Equals(string2));
            Assert.False(string2.Equals(string1));

            Assert.True(string1 != string2);
            Assert.True(string2 != string1);
        }

        [Fact]
        public void StringsWithSameContentHaveSameHashCode()
        {
            var string1 = new RedisString("test");
            var string2 = new RedisString("test");

            Assert.Equal(string1.GetHashCode(), string2.GetHashCode());
        }
    }
}