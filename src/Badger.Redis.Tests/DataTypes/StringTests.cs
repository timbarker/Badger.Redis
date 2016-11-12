using Badger.Redis.DataTypes;
using System;
using Xunit;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.Tests.DataTypes
{
    public class StringTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var s = new String("");

            Assert.Equal(DataType.String, s.DataType);
        }

        [Fact]
        public void CanImplicityConvertFromString()
        {
            String converted = "test";

            Assert.Equal("test", converted.Value);
        }

        [Fact]
        public void CanImplicityConvertToString()
        {
            string converted = new String("test");

            Assert.Equal("test", converted);
        }

        [Fact]
        public void ToStringCorrect()
        {
            var s = new String("test");

            Assert.Equal("test", s.ToString());
        }

        [Fact]
        public void ConstructingWithNullStringNotAllowed()
        {
            var ex = Assert.Throws<ArgumentException>(() => new String(null));

            Assert.Equal("value can't be null\r\nParameter name: value", ex.Message);
        }

        [Fact]
        public void StringsWithSameContentAreEqual()
        {
            var string1 = new String("test");
            var string2 = new String("test");

            Assert.True(string1.Equals(string2));
            Assert.True(string2.Equals(string1));
        }

        [Fact]
        public void StringsWithDifferentContentAreNotEqual()
        {
            var string1 = new String("test1");
            var string2 = new String("test2");

            Assert.False(string1.Equals(string2));
            Assert.False(string2.Equals(string1));
        }

        [Fact]
        public void StringsWithSameContentHaveSameHashCode()
        {
            var string1 = new String("test");
            var string2 = new String("test");

            Assert.Equal(string1.GetHashCode(), string2.GetHashCode());
        }
    }
}
