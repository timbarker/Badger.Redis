using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.Connection
{
    internal interface IConnectionCreator
    {
        Task<IConnection> CreateOpenedAsync(string host, int port, CancellationToken cancellationToken);
    }
}