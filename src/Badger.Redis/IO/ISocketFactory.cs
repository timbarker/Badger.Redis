using System.Net;

namespace Badger.Redis.IO
{
    public interface ISocketFactory
    {
        ISocket Create(IPEndPoint endPoint);
    }
}