using System.Net;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal interface IClientFactory
    {
        Task<IClient> CreateConnectedAsync(IPEndPoint endPoint);
    }
}