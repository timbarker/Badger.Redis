using Badger.Redis.Types;
using Xunit;

namespace Badger.Redis.Tests.Types
{
    public class RedisArrayTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var s = new RedisArray();

            Assert.Equal(RedisType.Array, s.DataType);
        }

        [Fact]
        public void ValueIsCorrect()
        {
            var a = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));

            Assert.Equal(new IRedisType[] { new RedisInteger(1), new RedisInteger(2), new RedisInteger(3) }, a.Value);
        }

        [Fact]
        public void EmptyArrayLenghtIsZero()
        {
            var a = new RedisArray();
            Assert.Equal(0, a.Length);
        }

        [Fact]
        public void EmptyArrayToString()
        {
            var a = new RedisArray();

            Assert.Equal("[]", a.ToString());
        }

        [Fact]
        public void NonEmptyArrayLengthCorrect()
        {
            var a = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));

            Assert.Equal(3, a.Length);
        }

        [Fact]
        public void NonEmptyArrayToStringIsCorrect()
        {
            var a = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));

            Assert.Equal("[1, 2, 3]", a.ToString());
        }

        [Fact]
        public void ConstructingWithNullAllowed()
        {
            var s = new RedisArray(null);

            Assert.Equal(RedisArray.Null, s);
        }

        [Fact]
        public void ArrayIsEqualToItself()
        {
            var array = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));

            Assert.True(array.Equals(array));
        }

        [Fact]
        public void ArrayIsNotEqualToNull()
        {
            var array = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));

            Assert.False(array.Equals(null));
        }

        [Fact]
        public void ArraysWithSameContentAreEqual()
        {
            var array1 = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));
            var array2 = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));

            Assert.True(array1.Equals(array2));
            Assert.True(array2.Equals(array1));

            Assert.True(array1 == array2);
            Assert.True(array2 == array1);
        }

        [Fact]
        public void ArraysWithDifferentContentAreNotEqual()
        {
            var array1 = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));
            var array2 = new RedisArray(new RedisInteger(3), new RedisInteger(2), new RedisInteger(1));

            Assert.False(array1.Equals(array2));
            Assert.False(array2.Equals(array1));

            Assert.True(array1 != array2);
            Assert.True(array2 != array1);
        }

        [Fact]
        public void ArraysWithDifferentLengthAreNotEqual()
        {
            var array1 = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));
            var array2 = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3), new RedisInteger(4));

            Assert.False(array1.Equals(array2));
            Assert.False(array2.Equals(array1));

            Assert.True(array1 != array2);
            Assert.True(array2 != array1);
        }

        [Fact]
        public void ArraysWithSameContentHaveSameHashCode()
        {
            var array1 = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));
            var array2 = new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3));

            Assert.Equal(array1.GetHashCode(), array2.GetHashCode());
        }

        [Fact]
        public void NullArrayToString()
        {
            var a = RedisArray.Null;

            Assert.Equal("", a.ToString());
        }

        [Fact]
        public void NullArrayGetHashCode()
        {
            var a = RedisArray.Null;

            Assert.Equal(0, a.GetHashCode());
        }

        [Fact]
        public void NullArraysAreEqualToNullArrays()
        {
            Assert.True(RedisArray.Null.Equals(RedisArray.Null));
        }

        [Fact]
        public void NullArraysLengthNegative1()
        {
            Assert.Equal(-1, RedisArray.Null.Length);
        }
    }
}