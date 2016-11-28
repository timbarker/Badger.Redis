using System.Net;

namespace Badger.Redis.Connection
{
    internal interface IConnectionFactory
    {
        IOpenableConnection Create(IPEndPoint endPoint);
    }
}