using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis
{
    /// <summary>
    /// Represents a connection to a Redis server.
    /// </summary>
    /// <remarks>
    /// Is not designed to be thread safe.
    /// </remarks>
    public interface IConnection : IDisposable
    {
        Task<string> PingAsync(CancellationToken token);
    }
}