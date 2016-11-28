using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.Connection
{
    internal interface IOpenableConnection : IConnection
    {
        Task OpenAsync(CancellationToken token);
    }
}