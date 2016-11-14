using Badger.Redis.DataTypes;
using Badger.Redis.IO;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Array = Badger.Redis.DataTypes.Array;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.Tests.IO
{
    public class ReaderTests : IDisposable
    {
        private Reader _reader;
        private Stream _stream;

        public ReaderTests()
        {
            _stream = new MemoryStream();
            _reader = new Reader(_stream);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        private void SetupStream(string data)
        {
            var dataBytes = Encoding.ASCII.GetBytes(data);
            _stream.Write(dataBytes, 0, dataBytes.Length);
            _stream.Seek(0, SeekOrigin.Begin);
        }

        private async Task<T> ReadAsync<T>(CancellationToken? cancellationToken = null) where T : IDataType
        {
            return (T)(await _reader.ReadAsync(cancellationToken ?? CancellationToken.None));
        }

        [Fact]
        public async Task ReadStringTest()
        {
            SetupStream("+OK\r\n");

            var result = await ReadAsync<String>();

            Assert.Equal(new String("OK"), result);
        }

        [Fact]
        public async Task ReadErrorTest()
        {
            SetupStream("-Error message\r\n");

            var result = await ReadAsync<Error>();

            Assert.Equal(new Error("Error message"), result);
        }

        [Fact]
        public async Task ReadIntegerTest()
        {
            SetupStream(":1000\r\n");

            var result = await ReadAsync<Integer>();

            Assert.Equal(1000, result.Value);
        }

        [Fact]
        public async Task ReadBulkStringTest()
        {
            SetupStream("$6\r\nfoobar\r\n");

            var result = await ReadAsync<BulkString>();

            Assert.Equal(BulkString.FromString("foobar"), result);
        }

        [Fact]
        public async Task ReadEmptyBulkStringTest()
        {
            SetupStream("$0\r\n\r\n");

            var result = await ReadAsync<BulkString>();

            Assert.Equal(BulkString.FromString(""), result);
        }

        [Fact]
        public async Task ReadNullBulkStringTest()
        {
            SetupStream("$-1\r\n");

            var result = await ReadAsync<BulkString>();

            Assert.Equal(BulkString.Null, result);
        }

        [Fact]
        public async Task ReadEmptyArrayTest()
        {
            SetupStream("*0\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(new Array(), result);
        }

        [Fact]
        public async Task ReadSingleElementArrayTest()
        {
            SetupStream("*1\r\n+OK\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(new Array(new String("OK")), result.Value);
        }

        [Fact]
        public async Task ReadIntegerArrayTest()
        {
            SetupStream("*3\r\n:1\r\n:2\r\n:3\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(new Array(new Integer(1), new Integer(2), new Integer(3)), result);
        }

        [Fact]
        public async Task ReadErrorArrayTest()
        {
            SetupStream("*1\r\n-Error message\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(new Array(new Error("Error message")), result);
        }

        [Fact]
        public async Task ReadBulkStringArrayTest()
        {
            SetupStream("*2\r\n$3\r\nfoo\r\n$3\r\nbar\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(new Array(BulkString.FromString("foo"), BulkString.FromString("bar")), result);
        }

        [Fact]
        public async Task ReadMixedArrayTest()
        {
            SetupStream("*5\r\n:1\r\n:2\r\n:3\r\n:4\r\n$6\r\nfoobar\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(new Array(new Integer(1), new Integer(2), new Integer(3), new Integer(4), BulkString.FromString("foobar")), result);
        }

        [Fact]
        public async Task ReadNullArrayTest()
        {
            SetupStream("*-1\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(Array.Null, result);
        }

        [Fact]
        public async Task ReadArrayOfArraysTest()
        {
            SetupStream("*2\r\n*3\r\n:1\r\n:2\r\n:3\r\n*2\r\n+Foo\r\n-Bar\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(new Array(
                            new Array(new Integer(1), new Integer(2), new Integer(3)),
                            new Array(new String("Foo"), new Error("Bar"))),
                         result);
        }

        [Fact]
        public async Task ReadArrayWithNullElementTest()
        {
            SetupStream("*3\r\n$3\r\nfoo\r\n$-1\r\n$3\r\nbar\r\n");

            var result = await ReadAsync<Array>();

            Assert.Equal(new Array(BulkString.FromString("foo"), BulkString.Null, BulkString.FromString("bar")), result);
        }

        [Theory]
        [InlineData("invalid\r\n", "Invalid prefix 'i'")]
        [InlineData(":hello\r\n", "Invalid Integer value 'hello'")]
        [InlineData("$-2\r\n", "Invalid BulkString length '-2'")]
        [InlineData("$hello\r\n", "Invalid BulkString length 'hello'")]
        [InlineData("*-2\r\n", "Invalid Array length '-2'")]
        [InlineData("*hello\r\n", "Invalid Array length 'hello'")]
        public async Task ReadInvalidDataTest(string data, string expectedMessage)
        {
            SetupStream(data);

            var ex = await Assert.ThrowsAsync<IOException>(async () => await ReadAsync<IDataType>());
            Assert.Equal(expectedMessage, ex.Message);
        }

        [Fact]
        public async Task ReadInvalidDataWithCancellation()
        {
            SetupStream("invalid");

            var cts = new CancellationTokenSource(50);
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await ReadAsync<IDataType>(cts.Token));
        }

        [Fact]
        public async Task MultipleMessageReadTest()
        {
            SetupStream("+OK\r\n+Test\r\n:100\r\n*3\r\n:1\r\n:2\r\n:3\r\n$6\r\nfoobar\r\n-Error message\r\n");

            IDataType result = await ReadAsync<String>();
            Assert.Equal(new String("OK"), result);

            result = await ReadAsync<String>();
            Assert.Equal(new String("Test"), result);

            result = await ReadAsync<Integer>();
            Assert.Equal(new Integer(100), result);

            result = await ReadAsync<Array>();
            Assert.Equal(new Array(new Integer(1), new Integer(2), new Integer(3)), result);

            result = await ReadAsync<BulkString>();
            Assert.Equal(BulkString.FromString("foobar"), result);

            result = await ReadAsync<Error>();
            Assert.Equal(new Error("Error message"), result);
        }
    }
}