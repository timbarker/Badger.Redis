using Badger.Redis.Connection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis
{
    /// <summary>
    /// A managed pool of connections to a Redis server.
    /// </summary>
    /// <remarks>
    /// Is designed to be thread safe.
    /// </remarks>
    public interface IConnectionPool : IDisposable
    {
        int ActiveConnections { get; }

        Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken);

        Task<IConnection> GetConnectionAsync();
    }
}