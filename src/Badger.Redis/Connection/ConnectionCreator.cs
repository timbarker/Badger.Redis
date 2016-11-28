using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.Connection
{
    internal class ConnectionCreator : IConnectionCreator
    {
        private readonly IConnectionFactory _connectionFactory;

        public ConnectionCreator(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IConnection> CreateOpenedAsync(string host, int port, CancellationToken cancellationToken)
        {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                throw new ConnectionException($"Port {port} outside of the range {IPEndPoint.MinPort} to {IPEndPoint.MaxPort}");

            var addresses = await GetHostNamesAsync(host);
            foreach (var address in addresses)
            {
                try
                {
                    var conn = _connectionFactory.Create(new IPEndPoint(address, port));
                    await conn.OpenAsync(cancellationToken);
                    return conn;
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            throw new ConnectionException($"Unable to establish a connection to {host}:{port}");
        }

        private async Task<IPAddress[]> GetHostNamesAsync(string host)
        {
            try
            {
                return await Dns.GetHostAddressesAsync(host);
            }
            catch (Exception ex)
            {
                throw new ConnectionException($"Unable to resolve host '{host}'", ex);
            }
        }
    }
}