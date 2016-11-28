using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal class ClientFactory : IClientFactory
    {
        public async Task<IClient> CreateConnectedAsync(IPEndPoint endPoint)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(endPoint.Address, endPoint.Port);
            return new Client(tcpClient);
        }
    }
}