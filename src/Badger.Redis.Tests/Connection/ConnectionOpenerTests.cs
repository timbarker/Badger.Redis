using Badger.Redis.Connection;
using Moq;
using System.Net;
using System.Threading;
using Xunit;

namespace Badger.Redis.Tests.Connection
{
    public class ConnectionOpenerTests
    {
        public class WhenOpeningAConnection
        {
            private Mock<IOpenableConnection> _connection;
            private ConnectionOpener _connectionOpener;
            private Mock<IConnectionFactory> _connectionFactory;

            public WhenOpeningAConnection()
            {
                _connectionFactory = new Mock<IConnectionFactory>();
                _connection = new Mock<IOpenableConnection>();
                _connectionFactory
                    .Setup(cf => cf.Create(It.IsAny<IPAddress>(), It.IsAny<int>()))
                    .Returns(_connection.Object);

                _connectionOpener = new ConnectionOpener(_connectionFactory.Object);

                _connectionOpener.OpenAsync("127.0.0.1", 1234, CancellationToken.None).Wait();
            }

            [Fact]
            public void ThenTheConnectionIsCreated()
            {
                _connectionFactory.Verify(cf => cf.Create(IPAddress.Loopback, 1234));
            }


            [Fact]
            public void ThenTheConnectionIsOpened()
            {
                _connection.Verify(c => c.OpenAsync(CancellationToken.None));
            }
        }        
    }
}
