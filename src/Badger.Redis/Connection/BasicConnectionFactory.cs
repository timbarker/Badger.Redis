using Badger.Redis.IO;
using System.Net;

namespace Badger.Redis.Connection
{
    internal class BasicConnectionFactory : IConnectionFactory
    {
        private readonly IClientFactory _clientFactory;

        public BasicConnectionFactory(IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IOpenableConnection Create(IPEndPoint ipEndPoint)
        {
            return new BasicConnection(ipEndPoint, _clientFactory);
        }
    }
}