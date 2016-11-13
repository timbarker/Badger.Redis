using Badger.Redis.DataTypes;
using Badger.Redis.IO;
using System.Threading.Tasks;
using Xunit;

namespace Badger.Redis.Tests.IO
{
    public class WriterTests
    {
        [Fact]
        public async Task WriteStringTest()
        {
            var result = await Writer.String.WriteStringAsync("OK");

            Assert.Equal("+OK\r\n", result);
        }

        [Fact]
        public async Task WriteErrorTest()
        {
            var result = await Writer.Error.WriteStringAsync(new Error("Error message"));

            Assert.Equal("-Error message\r\n", result);
        }

        [Fact]
        public async Task WriteIntegerTest()
        {
            var result = await Writer.Integer.WriteStringAsync(1000);

            Assert.Equal(":1000\r\n", result);
        }

        [Fact]
        public async Task WriteBulkStringTest()
        {
            var result = await Writer.BulkString.WriteStringAsync(BulkString.FromString("foobar"));

            Assert.Equal("$6\r\nfoobar\r\n", result);
        }

        [Fact]
        public async Task WriteEmptyBulkStringTest()
        {
            var result = await Writer.BulkString.WriteStringAsync(new BulkString(new byte[0]));

            Assert.Equal("$0\r\n\r\n", result);
        }

        [Fact]
        public async Task WriteNullBulkStringTest()
        {
            var result = await Writer.BulkString.WriteStringAsync(BulkString.Null);

            Assert.Equal("$-1\r\n", result);
        }

        [Fact]
        public async Task WriteEmptyArrayTest()
        {
            var result = await Writer.Array.WriteStringAsync(new Array());

            Assert.Equal("*0\r\n", result);
        }

        [Fact]
        public async Task WriteSingleElementArrayTest()
        {
            var result = await Writer.Array.WriteStringAsync(new Array(new String("OK")));

            Assert.Equal("*1\r\n+OK\r\n", result);
        }

        [Fact]
        public async Task WriteIntegerArrayTest()
        {
            var result = await Writer.Array.WriteStringAsync(new Array(new Integer(1), new Integer(2), new Integer(3)));

            Assert.Equal("*3\r\n:1\r\n:2\r\n:3\r\n", result);
        }

        [Fact]
        public async Task WriteErrorArrayTest()
        {
            var result = await Writer.Array.WriteStringAsync(new Array(new Error("Error message")));

            Assert.Equal("*1\r\n-Error message\r\n", result);
        }

        [Fact]
        public async Task WriteBulkStringArrayTest()
        {
            var result = await Writer.Array.WriteStringAsync(new Array(BulkString.FromString("foo"), BulkString.FromString("bar")));

            Assert.Equal("*2\r\n$3\r\nfoo\r\n$3\r\nbar\r\n", result);
        }

        [Fact]
        public async Task WriteMixedArrayTest()
        {
            var result = await Writer.Array.WriteStringAsync(new Array(new Integer(1), new Integer(2), new Integer(3), new Integer(4), BulkString.FromString("foobar")));

            Assert.Equal("*5\r\n:1\r\n:2\r\n:3\r\n:4\r\n$6\r\nfoobar\r\n", result);
        }

        [Fact]
        public async Task WriteNullArrayTest()
        {
            var result = await Writer.Array.WriteStringAsync(Array.Null);

            Assert.Equal("*-1\r\n", result);
        }

        [Fact]
        public async Task WriteArrayOfArraysTest()
        {
            var result = await Writer.Array.WriteStringAsync(new Array(
                                                            new Array(new Integer(1), new Integer(2), new Integer(3)),
                                                            new Array(new String("Foo"), new Error("Bar"))));

            Assert.Equal("*2\r\n*3\r\n:1\r\n:2\r\n:3\r\n*2\r\n+Foo\r\n-Bar\r\n", result);
        }

        [Fact]
        public async Task WriteArrayWithNullElementTest()
        {
            var result = await Writer.Array.WriteStringAsync(new Array(BulkString.FromString("foo"), BulkString.Null, BulkString.FromString("bar")));

            Assert.Equal("*3\r\n$3\r\nfoo\r\n$-1\r\n$3\r\nbar\r\n", result);
        }
    }
}
