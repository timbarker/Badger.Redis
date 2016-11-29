using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis
{
    public interface IConnection : IDisposable
    {
        Task<string> PingAsync(CancellationToken token);

        ConnectionState State { get; }
    }
}