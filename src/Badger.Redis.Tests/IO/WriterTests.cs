using Badger.Redis.IO;
using Badger.Redis.Types;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Badger.Redis.Tests.IO
{
    public class WriterTests : IDisposable
    {
        private Writer _writer;
        private MemoryStream _stream;

        public WriterTests()
        {
            _stream = new MemoryStream();
            _writer = new Writer(_stream);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private async Task<string> WriteAsync(IRedisType dataType)
        {
            await _writer.WriteAsync(dataType, CancellationToken.None);
            return Encoding.ASCII.GetString(_stream.ToArray());
        }

        [Fact]
        public async Task WriteStringTest()
        {
            var result = await WriteAsync(new RedisString("OK"));

            Assert.Equal("+OK\r\n", result);
        }

        [Fact]
        public async Task WriteErrorTest()
        {
            var result = await WriteAsync(new RedisErorr("Error message"));

            Assert.Equal("-Error message\r\n", result);
        }

        [Fact]
        public async Task WriteIntegerTest()
        {
            var result = await WriteAsync(new RedisInteger(1000));

            Assert.Equal(":1000\r\n", result);
        }

        [Fact]
        public async Task WriteBulkStringTest()
        {
            var result = await WriteAsync(RedisBulkString.FromString("foobar"));

            Assert.Equal("$6\r\nfoobar\r\n", result);
        }

        [Fact]
        public async Task WriteEmptyBulkStringTest()
        {
            var result = await WriteAsync(new RedisBulkString(new byte[0]));

            Assert.Equal("$0\r\n\r\n", result);
        }

        [Fact]
        public async Task WriteNullBulkStringTest()
        {
            var result = await WriteAsync(RedisBulkString.Null);

            Assert.Equal("$-1\r\n", result);
        }

        [Fact]
        public async Task WriteEmptyArrayTest()
        {
            var result = await WriteAsync(new RedisArray());

            Assert.Equal("*0\r\n", result);
        }

        [Fact]
        public async Task WriteSingleElementArrayTest()
        {
            var result = await WriteAsync(new RedisArray(new RedisString("OK")));

            Assert.Equal("*1\r\n+OK\r\n", result);
        }

        [Fact]
        public async Task WriteIntegerArrayTest()
        {
            var result = await WriteAsync(new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3)));

            Assert.Equal("*3\r\n:1\r\n:2\r\n:3\r\n", result);
        }

        [Fact]
        public async Task WriteErrorArrayTest()
        {
            var result = await WriteAsync(new RedisArray(new RedisErorr("Error message")));

            Assert.Equal("*1\r\n-Error message\r\n", result);
        }

        [Fact]
        public async Task WriteBulkStringArrayTest()
        {
            var result = await WriteAsync(new RedisArray(RedisBulkString.FromString("foo"), RedisBulkString.FromString("bar")));

            Assert.Equal("*2\r\n$3\r\nfoo\r\n$3\r\nbar\r\n", result);
        }

        [Fact]
        public async Task WriteMixedArrayTest()
        {
            var result = await WriteAsync(new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3), new RedisInteger(4), RedisBulkString.FromString("foobar")));

            Assert.Equal("*5\r\n:1\r\n:2\r\n:3\r\n:4\r\n$6\r\nfoobar\r\n", result);
        }

        [Fact]
        public async Task WriteNullArrayTest()
        {
            var result = await WriteAsync(RedisArray.Null);

            Assert.Equal("*-1\r\n", result);
        }

        [Fact]
        public async Task WriteArrayOfArraysTest()
        {
            var result = await WriteAsync(new RedisArray(
                                                    new RedisArray(new RedisInteger(1), new RedisInteger(2), new RedisInteger(3)),
                                                    new RedisArray(new RedisString("Foo"), new RedisErorr("Bar"))));

            Assert.Equal("*2\r\n*3\r\n:1\r\n:2\r\n:3\r\n*2\r\n+Foo\r\n-Bar\r\n", result);
        }

        [Fact]
        public async Task WriteArrayWithNullElementTest()
        {
            var result = await WriteAsync(new RedisArray(RedisBulkString.FromString("foo"), RedisBulkString.Null, RedisBulkString.FromString("bar")));

            Assert.Equal("*3\r\n$3\r\nfoo\r\n$-1\r\n$3\r\nbar\r\n", result);
        }
    }
}