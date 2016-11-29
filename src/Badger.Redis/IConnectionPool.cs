using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis
{
    public interface IConnectionPool : IDisposable
    {
        int ActiveConnections { get; }

        Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken);

        Task<IConnection> GetConnectionAsync();
    }
}