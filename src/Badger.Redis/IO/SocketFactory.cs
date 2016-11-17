using System.Net;

namespace Badger.Redis.IO
{
    public class SocketFactory : ISocketFactory
    {
        public ISocket Create(IPEndPoint endPoint)
        {
            return new Socket(endPoint);
        }
    }
}