using Badger.Redis.DataTypes;
using System;
using Xunit;

namespace Badger.Redis.Tests.DataTypes
{
    public class BulkStringTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var s = new BulkString(new byte[0]);

            Assert.Equal(DataType.BulkString, s.DataType);
        }

        [Fact]
        public void EmptyBulkStringLengthIs0()
        {
            var s = new BulkString(new byte[0]);

            Assert.Equal(0, s.Length);
        }

        [Fact]
        public void NonEmptyBulkStringLegnthIsCorrect()
        {
            var s = BulkString.FromString("test");

            Assert.Equal(4, s.Length);
        }

        [Fact]
        public void ToStringIsCorrect()
        {
            var s = BulkString.FromString("test");

            Assert.Equal("0x74657374", s.ToString());
        }

        [Fact]
        public void ConstructingWithMoreThan512MBOfDataNotAllowed()
        {
            var ex = Assert.Throws<ArgumentException>(() => new BulkString(new byte[512 * 1024 * 1024 + 1]));
            Assert.StartsWith("value is larger than 536870912 bytes", ex.Message);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void ConstructingWith512MBOfDataAllowed()
        {
            var s =  new BulkString(new byte[512 * 1024 * 1024]);

            Assert.Equal(536870912, s.Length);
        }

        [Fact]
        public void ConstructingWithNullStringAllowed()
        {
            var s = new BulkString(null);

            Assert.Equal(BulkString.Null, s);
        }

        [Fact]
        public void BulkStringsWithSameContentAreEqual()
        {
            var string1 = BulkString.FromString("test");
            var string2 = BulkString.FromString("test");

            Assert.True(string1.Equals(string2));
            Assert.True(string2.Equals(string1));
        }

        [Fact]
        public void BulkStringsWithDifferentContentAreNotEqual()
        {
            var string1 = BulkString.FromString("test1");
            var string2 = BulkString.FromString("test2");

            Assert.False(string1.Equals(string2));
            Assert.False(string2.Equals(string1));
        }

        [Fact]
        public void BulkStringsWithSameContentHaveSameHashCode()
        {
            var string1 = BulkString.FromString("test");
            var string2 = BulkString.FromString("test");

            Assert.Equal(string1.GetHashCode(), string2.GetHashCode());
        }

        [Fact]
        public void NullBulkStringToString()
        {
            var s = BulkString.Null;

            Assert.Equal("", s.ToString());
        }

        [Fact]
        public void NullBulkStringGetHashCode()
        {
            var s = BulkString.Null;

            Assert.Equal(0, s.GetHashCode());
        }

        [Fact]
        public void NullBulkStringEqualToNullBulkString()
        {
            Assert.True(BulkString.Null.Equals(BulkString.Null));
        }

        [Fact]
        public void NullBulkStringLengthNegative1()
        {
            Assert.Equal(-1, BulkString.Null.Length);
        }
    }
}
