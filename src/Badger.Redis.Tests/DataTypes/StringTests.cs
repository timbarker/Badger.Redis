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
        public void ValueIsCorrect()
        {
            var s = new String("test");

            Assert.Equal("test", s.Value);
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

            Assert.StartsWith("value can't be null", ex.Message);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void StringsAreEqualToItself()
        {
            var s = new String("test");

            Assert.True(s.Equals(s));
        }

        [Fact]
        public void StringsAreNotEqualToNull()
        {
            var s = new String("test");

            Assert.False(s.Equals(null));
        }

        [Fact]
        public void StringsWithSameContentAreEqual()
        {
            var string1 = new String("test");
            var string2 = new String("test");

            Assert.True(string1.Equals(string2));
            Assert.True(string2.Equals(string1));

            Assert.True(string1 == string2);
            Assert.True(string2 == string1);
        }

        [Fact]
        public void StringsWithDifferentContentAreNotEqual()
        {
            var string1 = new String("test1");
            var string2 = new String("test2");

            Assert.False(string1.Equals(string2));
            Assert.False(string2.Equals(string1));

            Assert.True(string1 != string2);
            Assert.True(string2 != string1);
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