using Badger.Redis.Types;
using System;
using Xunit;

namespace Badger.Redis.Tests.Types
{
    public class RedisBulkStringTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var s = new RedisBulkString(new byte[0]);

            Assert.Equal(RedisType.BulkString, s.DataType);
        }

        [Fact]
        public void ValueIsCorrect()
        {
            var s = RedisBulkString.FromString("test");

            Assert.Equal(new byte[] { 0x74, 0x65, 0x73, 0x74 }, s.Value);
        }

        [Fact]
        public void EmptyBulkStringLengthIs0()
        {
            var s = new RedisBulkString(new byte[0]);

            Assert.Equal(0, s.Length);
        }

        [Fact]
        public void NonEmptyBulkStringLegnthIsCorrect()
        {
            var s = RedisBulkString.FromString("test");

            Assert.Equal(4, s.Length);
        }

        [Fact]
        public void ToStringIsCorrect()
        {
            var s = RedisBulkString.FromString("test");

            Assert.Equal("0x74657374", s.ToString());
        }

        [Fact]
        public void ConstructingWithMoreThan512MBOfDataNotAllowed()
        {
            var ex = Assert.Throws<ArgumentException>(() => new RedisBulkString(new byte[512 * 1024 * 1024 + 1]));
            Assert.StartsWith("value is larger than 536870912 bytes", ex.Message);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void ConstructingWith512MBOfDataAllowed()
        {
            var s = new RedisBulkString(new byte[512 * 1024 * 1024]);

            Assert.Equal(536870912, s.Length);
        }

        [Fact]
        public void ConstructingWithNullStringAllowed()
        {
            var s = new RedisBulkString(null);

            Assert.Equal(RedisBulkString.Null, s);
        }

        [Fact]
        public void BulkStringsAreEqualToItself()
        {
            var s = RedisBulkString.FromString("test");

            Assert.True(s.Equals(s));
        }

        [Fact]
        public void BulkStringsAreNotEqualToNull()
        {
            var s = RedisBulkString.FromString("test");

            Assert.False(s.Equals(null));
        }

        [Fact]
        public void BulkStringsWithSameContentAreEqual()
        {
            var string1 = RedisBulkString.FromString("test");
            var string2 = RedisBulkString.FromString("test");

            Assert.True(string1.Equals(string2));
            Assert.True(string2.Equals(string1));

            Assert.True(string1 == string2);
            Assert.True(string2 == string1);
        }

        [Fact]
        public void BulkStringsWithDifferentContentAreNotEqual()
        {
            var string1 = RedisBulkString.FromString("test1");
            var string2 = RedisBulkString.FromString("test2");

            Assert.False(string1.Equals(string2));
            Assert.False(string2.Equals(string1));

            Assert.True(string1 != string2);
            Assert.True(string2 != string1);
        }

        [Fact]
        public void BulkStringsWithSameContentHaveSameHashCode()
        {
            var string1 = RedisBulkString.FromString("test");
            var string2 = RedisBulkString.FromString("test");

            Assert.Equal(string1.GetHashCode(), string2.GetHashCode());
        }

        [Fact]
        public void NullBulkStringToString()
        {
            var s = RedisBulkString.Null;

            Assert.Equal("", s.ToString());
        }

        [Fact]
        public void NullBulkStringGetHashCode()
        {
            var s = RedisBulkString.Null;

            Assert.Equal(0, s.GetHashCode());
        }

        [Fact]
        public void NullBulkStringEqualToNullBulkString()
        {
            Assert.True(RedisBulkString.Null.Equals(RedisBulkString.Null));
        }

        [Fact]
        public void NullBulkStringLengthNegative1()
        {
            Assert.Equal(-1, RedisBulkString.Null.Length);
        }
    }
}