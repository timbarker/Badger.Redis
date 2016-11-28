using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.Connection
{
    internal class ConnectionOpener : IConnectionOpener
    {
        private readonly IConnectionFactory _connectionFactory;

        public ConnectionOpener(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IConnection> OpenAsync(string host, int port, CancellationToken cancellationToken)
        {
            var conn = _connectionFactory.Create(IPAddress.Parse(host), port);
            await conn.OpenAsync(cancellationToken);
            return conn;
        }
    }
}