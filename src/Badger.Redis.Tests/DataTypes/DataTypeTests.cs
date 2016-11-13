using Badger.Redis.DataTypes;
using Xunit;

namespace Badger.Redis.Tests.DataTypes
{
    public class DataTypeTests
    {
        [Fact]
        public void StringPrefixTest()
        {
            Assert.Equal('+', DataType.String.Prefix());
        }

        [Fact]
        public void ErrorPrefixTest()
        {
            Assert.Equal('-', DataType.Error.Prefix());
        }

        [Fact]
        public void IntegerPrefixTest()
        {
            Assert.Equal(':', DataType.Integer.Prefix());
        }

        [Fact]
        public void BulkStringPrefixTest()
        {
            Assert.Equal('$', DataType.BulkString.Prefix());
        }

        [Fact]
        public void ArrayPrefixTest()
        {
            Assert.Equal('*', DataType.Array.Prefix());
        }
    }
}
