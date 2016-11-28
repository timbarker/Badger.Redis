using System.Net;

namespace Badger.Redis.Connection
{
    internal interface IConnectionFactory
    {
        IOpenableConnection Create(IPAddress address, int port);
    }
}