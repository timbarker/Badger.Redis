using Badger.Redis.Connection;
using Badger.Redis.IO;
using Badger.Redis.Types;
using Moq;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Badger.Redis.Tests.Connection
{
    public class BasicConnectionTests
    {
        public class WhenConnecting
        {
            private Mock<IClientFactory> _clientFactory;
            private Mock<IClient> _client;

            public WhenConnecting()
            {
                _clientFactory = new Mock<IClientFactory>();
                _client = new Mock<IClient>();
                _client.SetReturnsDefault(Task.FromResult<IRedisType>(new RedisString("PONG")));
                _clientFactory.SetReturnsDefault(Task.FromResult(_client.Object));

                var connection = new BasicConnection(new IPEndPoint(IPAddress.Loopback, 6379), _clientFactory.Object);

                connection.OpenAsync(CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenTheClientIsCreated()
            {
                _clientFactory.Verify(cf => cf.CreateConnectedAsync(It.Is<IPEndPoint>(ipe => ipe.Address == IPAddress.Loopback && ipe.Port == 6379)));
            }

            [Fact]
            public void ThenAPingMessageIsSent()
            {
                _client.Verify(c => c.SendAsync(It.Is<IRedisType>(dt => dt.DataType == RedisType.Array &&
                                                                        (dt as RedisArray).Cast<RedisBulkString>().First() == RedisBulkString.FromString("PING", Encoding.ASCII)),
                                                CancellationToken.None));
            }
        }

        public class WhenDisconnecting
        {
            private Mock<IClientFactory> _clientFactory;
            private Mock<IClient> _client;

            public WhenDisconnecting()
            {
                _clientFactory = new Mock<IClientFactory>();
                _client = new Mock<IClient>();
                _client.Setup(s => s.SendAsync(new RedisArray(RedisBulkString.FromString("PING", Encoding.ASCII)), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new RedisString("PONG"));
                _client.Setup(s => s.SendAsync(new RedisArray(RedisBulkString.FromString("QUIT", Encoding.ASCII)), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new RedisString("OK"));

                _clientFactory.SetReturnsDefault(Task.FromResult(_client.Object));

                var connection = new BasicConnection(new IPEndPoint(IPAddress.Loopback, 6379), _clientFactory.Object);

                connection.OpenAsync(CancellationToken.None).Wait();

                _client.ResetCalls();

                connection.DisconnectAsync(CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenTheClientIsDisposed()
            {
                _client.Verify(c => c.Dispose());
            }

            [Fact]
            public void ThenAQuitMessageIsSent()
            {
                _client.Verify(s => s.SendAsync(It.Is<IRedisType>(dt => dt.DataType == RedisType.Array &&
                                                                        (dt as RedisArray).Cast<RedisBulkString>().First() == RedisBulkString.FromString("QUIT", Encoding.ASCII)),
                                                CancellationToken.None));
            }
        }

        public class WhenSendingAPing
        {
            private Mock<IClientFactory> _clientFactory;
            private Mock<IClient> _client;

            public WhenSendingAPing()
            {
                _clientFactory = new Mock<IClientFactory>();
                _client = new Mock<IClient>();
                _client.SetReturnsDefault(Task.FromResult<IRedisType>(new RedisString("PONG")));
                _clientFactory.SetReturnsDefault(Task.FromResult(_client.Object));

                var connection = new BasicConnection(new IPEndPoint(IPAddress.Loopback, 6379), _clientFactory.Object);

                connection.OpenAsync(CancellationToken.None).Wait();

                _client.ResetCalls();

                connection.PingAsync(CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenAPingMessageIsSent()
            {
                _client.Verify(c => c.SendAsync(It.Is<IRedisType>(dt => dt.DataType == RedisType.Array &&
                                                                        (dt as RedisArray).Cast<RedisBulkString>().First() == RedisBulkString.FromString("PING", Encoding.ASCII)),
                                                CancellationToken.None));
            }
        }

        public class WhenAnErrorOccursWhenSendingAPing
        {
            private ConnectionException _ex;
            private BasicConnection _connection;
            private Mock<IClient> _client;

            public WhenAnErrorOccursWhenSendingAPing()
            {
                var clientFactory = new Mock<IClientFactory>();
                _client = new Mock<IClient>();
                _client
                    .Setup(c => c.SendAsync(It.IsAny<IRedisType>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new RedisString("PONG"));
                clientFactory.SetReturnsDefault(Task.FromResult(_client.Object));

                _connection = new BasicConnection(new IPEndPoint(IPAddress.Loopback, 6379), clientFactory.Object);

                _connection.OpenAsync(CancellationToken.None).Wait();

                _client.Reset();
                _client
                    .Setup(c => c.SendAsync(It.IsAny<IRedisType>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new SocketException());

                _ex = Assert.ThrowsAsync<ConnectionException>(() => _connection.PingAsync(CancellationToken.None)).Result;
            }

            [Fact]
            public void ThenTheExceptionMessageIsCorrect()
            {
                Assert.Equal("Error while making request", _ex.Message);
            }

            [Fact]
            public void ThenTheConnectionIsClosed()
            {
                Assert.Equal(ConnectionState.Closed, _connection.State);
            }

            [Fact]
            public void ThenTheClientIsDisposed()
            {
                _client.Verify(c => c.Dispose());
            }
        }
    }
}