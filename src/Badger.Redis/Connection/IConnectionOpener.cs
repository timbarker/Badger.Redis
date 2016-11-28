using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.Connection
{
    internal interface IConnectionOpener
    {
        Task<IConnection> OpenAsync(string host, int port, CancellationToken cancellationToken);
    }
}