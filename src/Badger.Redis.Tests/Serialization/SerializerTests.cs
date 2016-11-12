using Badger.Redis.DataTypes;
using Badger.Redis.Serialization;
using Xunit;

namespace Badger.Redis.Tests.Serialization
{
    public class SerializerTests
    {
        [Fact]
        public void StringSerializerTest()
        {
            var serialized = Serializer.String.Serialize("OK");

            Assert.Equal("+OK\r\n", serialized);
        }

        [Fact]
        public void ErrorSerializerTest()
        {
            var serialized = Serializer.Error.Serialize(new Error("Error message"));

            Assert.Equal("-Error message\r\n", serialized);
        }

        [Fact]
        public void IntegerSerializerTest()
        {
            var serialized = Serializer.Integer.Serialize(1000);

            Assert.Equal(":1000\r\n", serialized);
        }

        [Fact]
        public void BulkStringSerializerTest()
        {
            var serialized = Serializer.BulkString.Serialize("foobar");

            Assert.Equal("$6\r\nfoobar\r\n", serialized);
        }

        [Fact]
        public void EmptyBulkStringSerializerTest()
        {
            var serialized = Serializer.BulkString.Serialize("");

            Assert.Equal("$0\r\n\r\n", serialized);
        }

        [Fact]
        public void NullBulkStringSerializerTest()
        {
            var serialized = Serializer.BulkString.Serialize(BulkString.Null);

            Assert.Equal("$-1\r\n", serialized);
        }

        [Fact]
        public void EmptyArraySerializerTest()
        {
            var serialized = Serializer.Array.Serialize(new Array());

            Assert.Equal("*0\r\n", serialized);
        }

        [Fact]
        public void StringArraySerializerTest()
        {
            var serialized = Serializer.Array.Serialize(new Array(new String("OK")));

            Assert.Equal("*1\r\n+OK\r\n", serialized);
        }

        [Fact]
        public void IntegerArraySerializerTest()
        {
            var serialized = Serializer.Array.Serialize(new Array(new Integer(1), new Integer(2), new Integer(3)));

            Assert.Equal("*3\r\n:1\r\n:2\r\n:3\r\n", serialized);
        }

        [Fact]
        public void ErrorArraySerializerTest()
        {
            var serialized = Serializer.Array.Serialize(new Array(new Error("Error message")));

            Assert.Equal("*1\r\n-Error message\r\n", serialized);
        }

        [Fact]
        public void BulkStringArraySerializerTest()
        {
            var serialized = Serializer.Array.Serialize(new Array(new BulkString("foo"), new BulkString("bar")));

            Assert.Equal("*2\r\n$3\r\nfoo\r\n$3\r\nbar\r\n", serialized);
        }

        [Fact]
        public void MixedArraySerializerTest()
        {
            var serialized = Serializer.Array.Serialize(new Array(new Integer(1), new Integer(2), new Integer(3), new Integer(4), new BulkString("foobar")));

            Assert.Equal("*5\r\n:1\r\n:2\r\n:3\r\n:4\r\n$6\r\nfoobar\r\n", serialized);
        }

        [Fact]
        public void NullArraySerializerTest()
        {
            var serialized = Serializer.Array.Serialize(Array.Null);

            Assert.Equal("*-1\r\n", serialized);
        }

        [Fact]
        public void ArrayArraySerialierTest()
        {
            var serialized = Serializer.Array.Serialize(new Array(
                                                            new Array(new Integer(1), new Integer(2), new Integer(3)), 
                                                            new Array(new String("Foo"), new Error("Bar"))));

            Assert.Equal("*2\r\n*3\r\n:1\r\n:2\r\n:3\r\n*2\r\n+Foo\r\n-Bar\r\n", serialized);
        }

        [Fact]
        public void ArrayWithNullElementSerializerTest()
        {
            var serialized = Serializer.Array.Serialize(new Array(new BulkString("foo"), BulkString.Null, new BulkString("bar")));

            Assert.Equal("*3\r\n$3\r\nfoo\r\n$-1\r\n$3\r\nbar\r\n", serialized);
        }
    }
}
