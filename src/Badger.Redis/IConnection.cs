using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis
{
    public interface IConnection
    {
        Task ConnectAsync(CancellationToken cancellationToken);

        Task DisconnectAsync(CancellationToken cancellationToken);
    }
}