using Badger.Redis.IO;
using System.Net;

namespace Badger.Redis.Connection
{
    internal class BasicConnectionFactory : IConnectionFactory
    {
        private readonly ISocketFactory _socketFactory;

        public BasicConnectionFactory(ISocketFactory socketFactory)
        {
            _socketFactory = socketFactory;
        }

        public IOpenableConnection Create(IPAddress address, int port)
        {
            return new BasicConnection(address, port, _socketFactory);
        }
    }
}