using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Badger.Redis.IntegrationTests
{
    public class ConnectionTests
    {
        private static readonly Configuration Config = new Configuration { Host = "localhost", Port = 6379 };

        [Fact]
        public async Task WhenConnectedStateIsConnected()
        {
            using (var pool = new ConnectionPool(Config))
            {
                using (var connection = await pool.GetConnectionAsync())
                {
                    Assert.Equal(ConnectionState.Connected, connection.State);
                    await connection.PingAsync(CancellationToken.None);
                    Assert.Equal(1, pool.ActiveConnections);
                }

                Assert.Equal(0, pool.ActiveConnections);
            }
        }

        [Fact]
        public async Task WhenUnableToConnectAnExceptionIsThrown()
        {
            var brokenConfig = new Configuration { Host = Config.Host, Port = 1234 };

            using (var pool = new ConnectionPool(brokenConfig))
            {
                var ex = await Assert.ThrowsAsync<ConnectionException>(() => pool.GetConnectionAsync());

                Assert.Equal("Unable to establish a connection to localhost:1234", ex.Message);

                Assert.Equal(0, pool.ActiveConnections);
            }
        }

        [Fact]
        public async Task WhenConnectionPoolHasReachedMaxSizeAnExceptionIsThrown()
        {
            var brokenConfig = new Configuration { Host = Config.Host, Port = Config.Port, MaxPoolSize = 1 };

            using (var pool = new ConnectionPool(brokenConfig))
            using (var firstConnection = await pool.GetConnectionAsync())
            {
                var ex = await Assert.ThrowsAsync<ConnectionPoolException>(() => pool.GetConnectionAsync());

                Assert.Equal("Connection Pool size exceeded - MaxPoolSize: 1", ex.Message);

                Assert.Equal(1, pool.ActiveConnections);
            }
        }

        [Fact]
        public async Task OverlappedAsyncOperationsOnAConnection()
        {
            using (var pool = new ConnectionPool(Config))
            using (var connection = await pool.GetConnectionAsync())
            {
                var t1 = connection.PingAsync(CancellationToken.None);
                var t2 = connection.PingAsync(CancellationToken.None);

                await Task.WhenAll(t1, t2);
            }
        }
    }
}