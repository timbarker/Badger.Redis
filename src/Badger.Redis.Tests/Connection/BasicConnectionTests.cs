using Badger.Redis.Connection;
using Badger.Redis.DataTypes;
using Badger.Redis.IO;
using Moq;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Array = Badger.Redis.DataTypes.Array;
using String = Badger.Redis.DataTypes.String;

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
                _client.SetReturnsDefault(Task.FromResult<IDataType>(new String("PONG")));
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
                _client.Verify(c => c.SendAsync(It.Is<IDataType>(dt => dt.DataType == DataType.Array &&
                                                                        (dt as Array).Cast<BulkString>().First() == BulkString.FromString("PING", Encoding.ASCII)),
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
                _client.Setup(s => s.SendAsync(new Array(BulkString.FromString("PING", Encoding.ASCII)), It.IsAny<CancellationToken>()))
                       .Returns(Task.FromResult<IDataType>(new String("PONG")));
                _client.Setup(s => s.SendAsync(new Array(BulkString.FromString("QUIT", Encoding.ASCII)), It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult<IDataType>(new String("OK")));

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
                _client.Verify(s => s.SendAsync(It.Is<IDataType>(dt => dt.DataType == DataType.Array &&
                                                                        (dt as Array).Cast<BulkString>().First() == BulkString.FromString("QUIT", Encoding.ASCII)),
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
                _client.SetReturnsDefault(Task.FromResult<IDataType>(new String("PONG")));
                _clientFactory.SetReturnsDefault(Task.FromResult(_client.Object));

                var connection = new BasicConnection(new IPEndPoint(IPAddress.Loopback, 6379), _clientFactory.Object);

                connection.OpenAsync(CancellationToken.None).Wait();

                _client.ResetCalls();

                connection.PingAsync(CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenAPingMessageIsSent()
            {
                _client.Verify(c => c.SendAsync(It.Is<IDataType>(dt => dt.DataType == DataType.Array &&
                                                                        (dt as Array).Cast<BulkString>().First() == BulkString.FromString("PING", Encoding.ASCII)),
                                                CancellationToken.None));
            }
        }
    }
}