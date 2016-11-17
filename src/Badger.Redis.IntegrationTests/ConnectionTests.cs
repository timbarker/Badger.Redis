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
            var connectionFactory = new ConnectionFactory(Config);

            var connection = connectionFactory.Create();

            await connection.ConnectAsync(CancellationToken.None);

            await connection.DisconnectAsync(CancellationToken.None);
        }
    }
}