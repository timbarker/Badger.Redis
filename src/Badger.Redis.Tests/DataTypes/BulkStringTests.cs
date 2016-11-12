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
            var s = new BulkString("");

            Assert.Equal(DataType.BulkString, s.DataType);
        }

        [Fact]
        public void EmptyBulkStringLengthIs0()
        {
            var s = new BulkString("");

            Assert.Equal(0, s.Length);
        }

        [Fact]
        public void NonEmptyBulkStringLegnthIsCorrect()
        {
            var s = new BulkString("test");

            Assert.Equal(4, s.Length);
        }

        [Fact]
        public void CanImplicityConvertFromString()
        {
            BulkString converted = "test";

            Assert.Equal("test", converted.Value);
        }

        [Fact]
        public void CanImplicityConvertToString()
        {
            string converted = new BulkString("test");

            Assert.Equal("test", converted);
        }

        [Fact]
        public void ToStringIsCorrect()
        {
            var s = new BulkString("test");

            Assert.Equal("test", s.ToString());
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
            var string1 = new BulkString("test");
            var string2 = new BulkString("test");

            Assert.True(string1.Equals(string2));
            Assert.True(string2.Equals(string1));
        }

        [Fact]
        public void BulkStringsWithDifferentContentAreNotEqual()
        {
            var string1 = new BulkString("test1");
            var string2 = new BulkString("test2");

            Assert.False(string1.Equals(string2));
            Assert.False(string2.Equals(string1));
        }

        [Fact]
        public void BulkStringsWithSameContentHaveSameHashCode()
        {
            var string1 = new BulkString("test");
            var string2 = new BulkString("test");

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
        public void NullStringImplicityConvertsToNullBulkString()
        {
            BulkString converted = (string)null;
            Assert.Equal(BulkString.Null, converted);
        }

        [Fact]
        public void NullBulkStringImplicityConvertsToNullString()
        {
            string s = BulkString.Null;
            Assert.Null(s);
        }

        [Fact]
        public void NullBulkStringLengthNegative1()
        {
            Assert.Equal(-1, BulkString.Null.Length);
        }
    }
}
