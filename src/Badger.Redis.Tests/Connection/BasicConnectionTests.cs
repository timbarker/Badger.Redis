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
        public class GivenAConnectionWhenConnecting
        {
            private Mock<ISocketFactory> _socketFactory;
            private Mock<ISocket> _socket;

            public GivenAConnectionWhenConnecting()
            {
                _socketFactory = new Mock<ISocketFactory>();
                _socket = new Mock<ISocket>();
                _socket.SetReturnsDefault(Task.FromResult<IDataType>(new String("PONG")));
                _socketFactory.SetReturnsDefault(_socket.Object);

                var connection = new BasicConnection(IPAddress.Loopback, 6379, _socketFactory.Object);

                connection.OpenAsync(CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenTheSocketIsCreated()
            {
                _socketFactory.Verify(sf => sf.Create(It.Is<IPEndPoint>(ipe => ipe.Address == IPAddress.Loopback && ipe.Port == 6379)));
            }

            [Fact]
            public void ThenTheSocketIsOpened()
            {
                _socket.Verify(s => s.OpenAsync());
            }

            [Fact]
            public void ThenAPingMessageIsSent()
            {
                _socket.Verify(s => s.SendAsync(It.Is<IDataType>(dt => dt.DataType == DataType.Array &&
                                                                        (dt as Array).Cast<BulkString>().First() == BulkString.FromString("PING", Encoding.ASCII)),
                                                CancellationToken.None));
            }
        }

        public class GivenAConnectionWhenDisconnecting
        {
            private Mock<ISocketFactory> _socketFactory;
            private Mock<ISocket> _socket;

            public GivenAConnectionWhenDisconnecting()
            {
                _socketFactory = new Mock<ISocketFactory>();
                _socket = new Mock<ISocket>();
                _socket.Setup(s => s.SendAsync(new Array(BulkString.FromString("PING", Encoding.ASCII)), It.IsAny<CancellationToken>()))
                       .Returns(Task.FromResult<IDataType>(new String("PONG")));
                _socket.Setup(s => s.SendAsync(new Array(BulkString.FromString("QUIT", Encoding.ASCII)), It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult<IDataType>(new String("OK")));

                _socketFactory.SetReturnsDefault(_socket.Object);

                var connection = new BasicConnection(IPAddress.Loopback, 6379, _socketFactory.Object);

                connection.OpenAsync(CancellationToken.None).Wait();

                _socket.ResetCalls();

                connection.DisconnectAsync(CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenTheSocketIsClosed()
            {
                _socket.Verify(s => s.Close());
            }

            [Fact]
            public void ThenAPingMessageIsSent()
            {
                _socket.Verify(s => s.SendAsync(It.Is<IDataType>(dt => dt.DataType == DataType.Array &&
                                                                        (dt as Array).Cast<BulkString>().First() == BulkString.FromString("QUIT", Encoding.ASCII)),
                                                CancellationToken.None));
            }
        }
    }
}