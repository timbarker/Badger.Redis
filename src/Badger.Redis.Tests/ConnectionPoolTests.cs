using Badger.Redis.Connection;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Badger.Redis.Tests
{
    public class ConnectionPoolTests
    {
        public class WhenGettingAConnectionFromAnEmptyPool
        {
            private Mock<IConnectionCreator> _connectionFactory;
            private Configuration _config = new Configuration();
            private ConnectionPool _connectionPool;

            public WhenGettingAConnectionFromAnEmptyPool()
            {
                _connectionFactory = new Mock<IConnectionCreator>();
                _connectionPool = new ConnectionPool(_config, _connectionFactory.Object);

                var connection = _connectionPool.GetConnectionAsync().Result;
            }

            [Fact]
            public void ThenANewConnectionIsOpened()
            {
                _connectionFactory.Verify(cf => cf.CreateOpenedAsync(_config.Host, _config.Port, CancellationToken.None));
            }

            [Fact]
            public void ThenThePoolHasOneActiveConnection()
            {
                Assert.Equal(1, _connectionPool.ActiveConnections);
            }
        }

        public class WhenReturningAConnectionToThePool
        {
            private Mock<IConnectionCreator> _connectionFactory;
            private Configuration _config = new Configuration();
            private ConnectionPool _connectionPool;
            private Mock<IConnection> _connection;

            public WhenReturningAConnectionToThePool()
            {
                _connection = new Mock<IConnection>();
                _connectionFactory = new Mock<IConnectionCreator>();
                _connectionFactory
                    .Setup(cf => cf.CreateOpenedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(_connection.Object));
                _connectionPool = new ConnectionPool(_config, _connectionFactory.Object);

                _connectionPool.GetConnectionAsync().Result.Dispose();
            }

            [Fact]
            public void ThenTheConnectionIsNotDisposed()
            {
                _connection.Verify(c => c.Dispose(), Times.Never());
            }

            [Fact]
            public void ThenThePoolHasNoActiveConnections()
            {
                Assert.Equal(0, _connectionPool.ActiveConnections);
            }
        }

        public class WhenGettingAConnectionFromAnNonEmptyPool
        {
            private Mock<IConnectionCreator> _connectionFactory;
            private Configuration _config = new Configuration();
            private ConnectionPool _connectionPool;
            private Mock<IConnection> _connection;

            public WhenGettingAConnectionFromAnNonEmptyPool()
            {
                _connection = new Mock<IConnection>();
                _connectionFactory = new Mock<IConnectionCreator>();
                _connectionFactory
                            .Setup(cf => cf.CreateOpenedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                            .Returns(Task.FromResult(_connection.Object));

                _connectionPool = new ConnectionPool(_config, _connectionFactory.Object);
                _connectionPool.GetConnectionAsync().Result.Dispose();

                _connectionFactory.ResetCalls();

                var connection = _connectionPool.GetConnectionAsync().Result;
            }

            [Fact]
            public void ThenTheExistingConnectionIsReused()
            {
                _connectionFactory.Verify(cf => cf.CreateOpenedAsync(It.IsAny<string>(), It.IsAny<int>(), CancellationToken.None), Times.Never);
            }

            [Fact]
            public void ThenThePoolHasOneActiveConnection()
            {
                Assert.Equal(1, _connectionPool.ActiveConnections);
            }

            [Fact]
            public void ThenTheConnectionIsPinged()
            {
                _connection.Verify(c => c.PingAsync(CancellationToken.None));
            }
        }

        public class WhenGettingAConnectionFromAPoolThatIsReachedItsConnectionLimit
        {
            private Mock<IConnectionCreator> _connectionFactory;
            private Configuration _config = new Configuration();
            private ConnectionPool _connectionPool;
            private Exception _exception;

            public WhenGettingAConnectionFromAPoolThatIsReachedItsConnectionLimit()
            {
                _config.MaxPoolSize = 1;
                _connectionFactory = new Mock<IConnectionCreator>();

                _connectionPool = new ConnectionPool(_config, _connectionFactory.Object);
                _connectionPool.GetConnectionAsync().Wait();

                _connectionFactory.ResetCalls();

                _exception = Assert.ThrowsAsync<ConnectionPoolException>(() => _connectionPool.GetConnectionAsync()).Result;
            }

            [Fact]
            public void ThenTheExceptionMessageIsCorrect()
            {
                Assert.Equal("Connection Pool size exceeded - MaxPoolSize: 1", _exception.Message);
            }

            [Fact]
            public void ThenNoNewConnectionIsMade()
            {
                _connectionFactory.Verify(cf => cf.CreateOpenedAsync(It.IsAny<string>(), It.IsAny<int>(), CancellationToken.None), Times.Never);
            }

            [Fact]
            public void ThenThePoolHasOneActiveConnection()
            {
                Assert.Equal(1, _connectionPool.ActiveConnections);
            }
        }

        public class WhenDisposingAConnectionPoolWithPooledConnections
        {
            private Mock<IConnectionCreator> _connectionFactory;
            private Configuration _config = new Configuration();
            private ConnectionPool _connectionPool;
            private Mock<IConnection> _connection;

            public WhenDisposingAConnectionPoolWithPooledConnections()
            {
                _connection = new Mock<IConnection>();
                _connectionFactory = new Mock<IConnectionCreator>();
                _connectionFactory
                            .Setup(cf => cf.CreateOpenedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                            .Returns(Task.FromResult(_connection.Object));

                _connectionPool = new ConnectionPool(_config, _connectionFactory.Object);
                _connectionPool.GetConnectionAsync().Result.Dispose();

                _connectionPool.Dispose();
            }

            [Fact]
            public void ThenThePooledConnectionIsDisposed()
            {
                _connection.Verify(c => c.Dispose());
            }
        }

        public class WhenDisposingAConnectionPoolWithActiveConnections
        {
            private Mock<IConnectionCreator> _connectionFactory;
            private Configuration _config = new Configuration();
            private ConnectionPool _connectionPool;
            private Mock<IConnection> _connection;

            public WhenDisposingAConnectionPoolWithActiveConnections()
            {
                _connection = new Mock<IConnection>();
                _connectionFactory = new Mock<IConnectionCreator>();
                _connectionFactory
                            .Setup(cf => cf.CreateOpenedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                            .Returns(Task.FromResult(_connection.Object));

                _connectionPool = new ConnectionPool(_config, _connectionFactory.Object);
                _connectionPool.GetConnectionAsync().Wait();

                _connectionPool.Dispose();
            }

            [Fact]
            public void ThenThePooledConnectionIsNotDisposed()
            {
                _connection.Verify(c => c.Dispose(), Times.Never);
            }
        }

        public class WhenReturningAnActiveConnectionToADisposedPool
        {
            private Mock<IConnectionCreator> _connectionFactory;
            private Configuration _config = new Configuration();
            private ConnectionPool _connectionPool;
            private Mock<IConnection> _connection;

            public WhenReturningAnActiveConnectionToADisposedPool()
            {
                _connection = new Mock<IConnection>();
                _connectionFactory = new Mock<IConnectionCreator>();
                _connectionFactory
                            .Setup(cf => cf.CreateOpenedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                            .Returns(Task.FromResult(_connection.Object));

                _connectionPool = new ConnectionPool(_config, _connectionFactory.Object);
                var activeConnection = _connectionPool.GetConnectionAsync().Result;

                _connectionPool.Dispose();

                activeConnection.Dispose();
            }

            [Fact]
            public void ThenTheActiveConnectionIsDisposed()
            {
                _connection.Verify(c => c.Dispose());
            }
        }
    }
}