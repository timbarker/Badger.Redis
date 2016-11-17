using System.Threading;
using System.Threading.Tasks;
using System;

namespace Badger.Redis
{
    public interface IConnection : IDisposable
    {
        Task<string> PingAsync(CancellationToken token);
    }
}