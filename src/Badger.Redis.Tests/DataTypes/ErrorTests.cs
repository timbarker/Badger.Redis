using Badger.Redis.DataTypes;
using System;
using Xunit;

namespace Badger.Redis.Tests.DataTypes
{
    public class ErrorTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var e = new Error("");

            Assert.Equal(DataType.Error, e.DataType);
        }

        [Fact]
        public void ToStringCorrect()
        {
            var e = new Error("error message");

            Assert.Equal("error message", e.ToString());
        }

        [Fact]
        public void ConstructingWithNullStringNotAllowed()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Error(null));

            Assert.StartsWith("value can't be null", ex.Message);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void ErrorsWithSameContentAreEqual()
        {
            var error1 = new Error("test");
            var error2 = new Error("test");

            Assert.True(error1.Equals(error2));
            Assert.True(error2.Equals(error1));
        }

        [Fact]
        public void ErrorsWithDifferentContentAreNotEqual()
        {
            var error1 = new Error("test1");
            var error2 = new Error("test2");

            Assert.False(error1.Equals(error2));
            Assert.False(error2.Equals(error1));
        }

        [Fact]
        public void ErrorsWithSameContentHaveSameHashCode()
        {
            var error1 = new Error("test");
            var error2 = new Error("test");

            Assert.Equal(error1.GetHashCode(), error2.GetHashCode());
        }
    }
}
