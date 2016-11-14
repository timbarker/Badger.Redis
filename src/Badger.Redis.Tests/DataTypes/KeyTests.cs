using Badger.Redis.DataTypes;
using System;
using Xunit;

namespace Badger.Redis.Tests.DataTypes
{
    public class KeyTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var key = new Key("test");

            Assert.Equal(DataType.BulkString, key.DataType);
        }

        [Fact]
        public void ValueIsCorrect()
        {
            var k = new Key("test");

            Assert.Equal(new byte[] { 0x74, 0x65, 0x73, 0x74 }, k.Value);
        }

        [Fact]
        public void ToStringIsCorrect()
        {
            var key = new Key("test");

            Assert.Equal("test", key.ToString());
        }

        [Fact]
        public void KeyWithObjectIdAndStringIdToStringIsCorrect()
        {
            var key = new Key("object", "test");

            Assert.Equal("object:test", key.ToString());
        }

        [Fact]
        public void KeyWithObjectIdAndLongIdToStringIsCorrect()
        {
            var key = new Key("object", 1234L);

            Assert.Equal("object:1234", key.ToString());
        }

        [Fact]
        public void KeyWithObjectIdAndGuidIdToStringIsCorrect()
        {
            var id = Guid.Parse("d551622c-ca4b-4fdc-b0ee-8fe3862961c5");
            var key = new Key("object", id);

            Assert.Equal("object:d551622c-ca4b-4fdc-b0ee-8fe3862961c5", key.ToString());
        }

        [Fact]
        public void KeyWithCustomSeperatorToStringIsCorrect()
        {
            var key = new Key('.', "test", 12345);

            Assert.Equal("test.12345", key.ToString());
        }

        [Fact]
        public void KeysAreEqualToItself()
        {
            var key = new Key("test");

            Assert.True(key.Equals(key));
        }

        [Fact]
        public void KeysAreNotEqualToNull()
        {
            var key = new Key("test");

            Assert.False(key.Equals(null));
        }

        [Fact]
        public void BulkStringsWithSameContentAreEqual()
        {
            var key1 = new Key("test");
            var key2 = new Key("test");

            Assert.True(key1.Equals(key2));
            Assert.True(key2.Equals(key1));

            Assert.True(key1 == key2);
            Assert.True(key2 == key1);
        }

        [Fact]
        public void KeysWithDifferentContentAreNotEqual()
        {
            var key1 = new Key("test1");
            var key2 = new Key("test2");

            Assert.False(key1.Equals(key2));
            Assert.False(key2.Equals(key1));

            Assert.True(key1 != key2);
            Assert.True(key2 != key1);
        }

        [Fact]
        public void KeyssWithSameContentHaveSameHashCode()
        {
            var key1 = new Key("test");
            var key2 = new Key("test");

            Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
        }
    }
}