using Badger.Redis.DataTypes;
using Xunit;
using Array = Badger.Redis.DataTypes.Array;

namespace Badger.Redis.Tests.DataTypes
{
    public class ArrayTests
    {
        [Fact]
        public void DataTypeIsCorrect()
        {
            var s = new Array();

            Assert.Equal(DataType.Array, s.DataType);
        }

        [Fact]
        public void EmptyArrayLenghtIsZero()
        {
            var a = new Array();
            Assert.Equal(0, a.Length);
        }

        [Fact]
        public void EmptyArrayToString()
        {
            var a = new Array();

            Assert.Equal("[]", a.ToString());
        }

        [Fact]
        public void NonEmptyArrayLengthCorrect()
        {
            var a = new Array(new Integer(1), new Integer(2), new Integer(3));

            Assert.Equal(3, a.Length);
        }

        [Fact]
        public void NonEmptyArrayToStringIsCorrect()
        {
            var a = new Array(new Integer(1), new Integer(2), new Integer(3));

            Assert.Equal("[1, 2, 3]", a.ToString());
        }

        [Fact]
        public void ConstructingWithNullAllowed()
        {
            var s = new Array(null);

            Assert.Equal(Array.Null, s);
        }

        [Fact]
        public void ArraysWithSameContentAreEqual()
        {
            var array1 = new Array(new Integer(1), new Integer(2), new Integer(3));
            var array2 = new Array(new Integer(1), new Integer(2), new Integer(3));

            Assert.True(array1.Equals(array2));
            Assert.True(array2.Equals(array1));
        }

        [Fact]
        public void ArraysWithDifferentContentAreNotEqual()
        {
            var array1 = new Array(new Integer(1), new Integer(2), new Integer(3));
            var array2 = new Array(new Integer(3), new Integer(2), new Integer(1));
            
            Assert.False(array1.Equals(array2));
            Assert.False(array2.Equals(array1));
        }

        [Fact]
        public void ArraysWithDifferentLengthAreNotEqual()
        {
            var array1 = new Array(new Integer(1), new Integer(2), new Integer(3));
            var array2 = new Array(new Integer(1), new Integer(2), new Integer(3), new Integer(4));

            Assert.False(array1.Equals(array2));
            Assert.False(array2.Equals(array1));
        }

        [Fact]
        public void ArraysWithSameContentHaveSameHashCode()
        {
            var array1 = new Array(new Integer(1), new Integer(2), new Integer(3));
            var array2 = new Array(new Integer(1), new Integer(2), new Integer(3));

            Assert.Equal(array1.GetHashCode(), array2.GetHashCode());
        }

        [Fact]
        public void NullArrayToString()
        {
            var a = Array.Null;

            Assert.Equal("", a.ToString());
        }

        [Fact]
        public void NullArrayGetHashCode()
        {
            var a = Array.Null;

            Assert.Equal(0, a.GetHashCode());
        }

        [Fact]
        public void NullArraysAreEqualToNullArrays()
        {
            Assert.True(Array.Null.Equals(Array.Null));
        }

        [Fact]
        public void NullArraysLengthNegative1()
        {
            Assert.Equal(-1, Array.Null.Length);
        }
    }
}
