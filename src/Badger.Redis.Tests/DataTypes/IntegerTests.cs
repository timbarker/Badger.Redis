using Badger.Redis.DataTypes;
using Xunit;

namespace Badger.Redis.Tests.DataTypes
{
    public class IntegerTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var i = new Integer(0);

            Assert.Equal(DataType.Integer, i.DataType);
        }

        [Fact]
        public void CanImplicityConvertFromLong()
        {
            Integer converted = 1234L;

            Assert.Equal(1234L, converted.Value);
        }

        [Fact]
        public void CanImplicityConvertToLong()
        {
            long converted = new Integer(1234);

            Assert.Equal(1234L, converted);
        }

        [Fact]
        public void ToStringCorrect()
        {
            var i = new Integer(1234);

            Assert.Equal("1234", i.ToString());
        }

        [Fact]
        public void IntegersWithSameValueAreEqual()
        {
            var integer1 = new Integer(1234);
            var integer2 = new Integer(1234);

            Assert.True(integer1.Equals(integer2));
            Assert.True(integer2.Equals(integer1));
        }

        [Fact]
        public void IntegersWithDifferentValueAreNotEqual()
        {
            var integer1 = new Integer(1234);
            var integer2 = new Integer(4321);

            Assert.False(integer1.Equals(integer2));
            Assert.False(integer2.Equals(integer1));
        }

        [Fact]
        public void IntegersWithSameContentHaveSameHashCode()
        {
            var integer1 = new Integer(1234);
            var integer2 = new Integer(1234);

            Assert.Equal(integer1.GetHashCode(), integer2.GetHashCode());
        }
    }
}
