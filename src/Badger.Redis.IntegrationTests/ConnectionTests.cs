using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Badger.Redis.IntegrationTests
{
    public class ConnectionTests
    {
        private static readonly Configuration Config = new Configuration { Address = "127.0.0.1", Port = 6379 };

        [Fact]
        public async Task CanConnectAndDisconnect()
        {
            var pool = new ConnectionPool(Config);

            using (var connection = await pool.CreateAsync(CancellationToken.None))
            {
                await connection.PingAsync(CancellationToken.None);
            }
        }
    }
}