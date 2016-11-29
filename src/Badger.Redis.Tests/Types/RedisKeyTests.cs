using Badger.Redis.Types;
using System;
using Xunit;

namespace Badger.Redis.Tests.Types
{
    public class RedisKeyTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var key = new RedisKey("test");

            Assert.Equal(RedisType.BulkString, key.RedisType);
        }

        [Fact]
        public void ValueIsCorrect()
        {
            var k = new RedisKey("test");

            Assert.Equal(new byte[] { 0x74, 0x65, 0x73, 0x74 }, k.Value);
        }

        [Fact]
        public void ToStringIsCorrect()
        {
            var key = new RedisKey("test");

            Assert.Equal("test", key.ToString());
        }

        [Fact]
        public void KeyWithObjectIdAndStringIdToStringIsCorrect()
        {
            var key = new RedisKey("object", "test");

            Assert.Equal("object:test", key.ToString());
        }

        [Fact]
        public void KeyWithObjectIdAndLongIdToStringIsCorrect()
        {
            var key = new RedisKey("object", 1234L);

            Assert.Equal("object:1234", key.ToString());
        }

        [Fact]
        public void KeyWithObjectIdAndGuidIdToStringIsCorrect()
        {
            var id = Guid.Parse("d551622c-ca4b-4fdc-b0ee-8fe3862961c5");
            var key = new RedisKey("object", id);

            Assert.Equal("object:d551622c-ca4b-4fdc-b0ee-8fe3862961c5", key.ToString());
        }

        [Fact]
        public void KeyWithCustomSeperatorToStringIsCorrect()
        {
            var key = new RedisKey('.', "test", 12345);

            Assert.Equal("test.12345", key.ToString());
        }

        [Fact]
        public void KeysAreEqualToItself()
        {
            var key = new RedisKey("test");

            Assert.True(key.Equals(key));
        }

        [Fact]
        public void KeysAreNotEqualToNull()
        {
            var key = new RedisKey("test");

            Assert.False(key.Equals(null));
        }

        [Fact]
        public void BulkStringsWithSameContentAreEqual()
        {
            var key1 = new RedisKey("test");
            var key2 = new RedisKey("test");

            Assert.True(key1.Equals(key2));
            Assert.True(key2.Equals(key1));

            Assert.True(key1 == key2);
            Assert.True(key2 == key1);
        }

        [Fact]
        public void KeysWithDifferentContentAreNotEqual()
        {
            var key1 = new RedisKey("test1");
            var key2 = new RedisKey("test2");

            Assert.False(key1.Equals(key2));
            Assert.False(key2.Equals(key1));

            Assert.True(key1 != key2);
            Assert.True(key2 != key1);
        }

        [Fact]
        public void KeyssWithSameContentHaveSameHashCode()
        {
            var key1 = new RedisKey("test");
            var key2 = new RedisKey("test");

            Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
        }
    }
}