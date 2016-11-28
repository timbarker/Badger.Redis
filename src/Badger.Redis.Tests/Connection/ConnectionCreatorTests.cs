using Badger.Redis.Connection;
using Moq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Badger.Redis.Tests.Connection
{
    public class ConnectionCreatorTests
    {
        public class WhenCreatingAnOpenedConnectionWithAnV4IpAddress
        {
            private Mock<IOpenableConnection> _connection;
            private ConnectionCreator _connectionOpener;
            private Mock<IConnectionFactory> _connectionFactory;

            public WhenCreatingAnOpenedConnectionWithAnV4IpAddress()
            {
                _connectionFactory = new Mock<IConnectionFactory>();
                _connection = new Mock<IOpenableConnection>();
                _connectionFactory
                    .Setup(cf => cf.Create(It.IsAny<IPEndPoint>()))
                    .Returns(_connection.Object);

                _connectionOpener = new ConnectionCreator(_connectionFactory.Object);

                _connectionOpener.CreateOpenedAsync("127.0.0.1", 1234, CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenTheConnectionIsCreated()
            {
                _connectionFactory.Verify(cf => cf.Create(It.Is<IPEndPoint>(ipe => ipe.Address.Equals(IPAddress.Loopback) && ipe.Port == 1234)));
            }

            [Fact]
            public void ThenTheConnectionIsOpened()
            {
                _connection.Verify(c => c.OpenAsync(CancellationToken.None));
            }
        }

        public class WhenCreatingAnOpenedConnectionWithAnV6IpAddress
        {
            private Mock<IOpenableConnection> _connection;
            private ConnectionCreator _connectionOpener;
            private Mock<IConnectionFactory> _connectionFactory;

            public WhenCreatingAnOpenedConnectionWithAnV6IpAddress()
            {
                _connectionFactory = new Mock<IConnectionFactory>();
                _connection = new Mock<IOpenableConnection>();
                _connectionFactory
                    .Setup(cf => cf.Create(It.IsAny<IPEndPoint>()))
                    .Returns(_connection.Object);

                _connectionOpener = new ConnectionCreator(_connectionFactory.Object);

                _connectionOpener.CreateOpenedAsync("::1", 1234, CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenTheConnectionIsCreated()
            {
                _connectionFactory.Verify(cf => cf.Create(It.Is<IPEndPoint>(ipe => ipe.Address.Equals(IPAddress.IPv6Loopback) && ipe.Port == 1234)));
            }


            [Fact]
            public void ThenTheConnectionIsOpened()
            {
                _connection.Verify(c => c.OpenAsync(CancellationToken.None));
            }
        }

        public class WhenCreatingAnOpenedConnectionWithAHostName
        {
            private Mock<IOpenableConnection> _connection;
            private ConnectionCreator _connectionOpener;
            private Mock<IConnectionFactory> _connectionFactory;

            public WhenCreatingAnOpenedConnectionWithAHostName()
            {
                _connectionFactory = new Mock<IConnectionFactory>();
                _connection = new Mock<IOpenableConnection>();
                _connectionFactory
                    .Setup(cf => cf.Create(It.IsAny<IPEndPoint>()))
                    .Returns(_connection.Object);

                _connectionOpener = new ConnectionCreator(_connectionFactory.Object);

                _connectionOpener.CreateOpenedAsync("localhost", 1234, CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenTheConnectionIsCreated()
            {
                _connectionFactory.Verify(cf => cf.Create(It.Is<IPEndPoint>(ipe => ipe.Address.Equals(IPAddress.IPv6Loopback) && ipe.Port == 1234)));
            }


            [Fact]
            public void ThenTheConnectionIsOpened()
            {
                _connection.Verify(c => c.OpenAsync(CancellationToken.None));
            }
        }

        public class WhenCreatingAnOpenedConnectionWithAnInvalidHostName
        {
            private Mock<IOpenableConnection> _connection;
            private ConnectionCreator _connectionOpener;
            private Mock<IConnectionFactory> _connectionFactory;

            public WhenCreatingAnOpenedConnectionWithAnInvalidHostName()
            {
                _connectionFactory = new Mock<IConnectionFactory>();
                _connection = new Mock<IOpenableConnection>();
                _connectionFactory
                    .Setup(cf => cf.Create(It.IsAny<IPEndPoint>()))
                    .Returns(_connection.Object);

                _connectionOpener = new ConnectionCreator(_connectionFactory.Object);
            }

            [Fact]
            public async Task ThenAnExceptionIsThrown()
            {
                var ex = await Assert.ThrowsAsync<ConnectionException>(() => _connectionOpener.CreateOpenedAsync("invalid.host.name.", 1234, CancellationToken.None));

                Assert.Equal("Unable to resolve host 'invalid.host.name.'", ex.Message);
            }
        }

        public class WhenCreatingAnOpenedConnectionWithAnInvalidPort
        {
            private Mock<IOpenableConnection> _connection;
            private ConnectionCreator _connectionOpener;
            private Mock<IConnectionFactory> _connectionFactory;

            public WhenCreatingAnOpenedConnectionWithAnInvalidPort()
            {
                _connectionFactory = new Mock<IConnectionFactory>();
                _connection = new Mock<IOpenableConnection>();
                _connectionFactory
                    .Setup(cf => cf.Create(It.IsAny<IPEndPoint>()))
                    .Returns(_connection.Object);

                _connectionOpener = new ConnectionCreator(_connectionFactory.Object);
            }

            [Fact]
            public async Task ThenAnExceptionIsThrown()
            {
                var ex = await Assert.ThrowsAsync<ConnectionException>(() => _connectionOpener.CreateOpenedAsync("localhost", 70000, CancellationToken.None));

                Assert.Equal("Port 70000 outside of the range 0 to 65535", ex.Message);
            }
        }
    }
}
