using Badger.Redis.IO;
using System.Net;

namespace Badger.Redis
{
    public class ConnectionFactory
    {
        private ISocketFactory _socketFactory;
        private Configuration _configuration;

        public ConnectionFactory(Configuration configuration)
        {
            _configuration = configuration;
            _socketFactory = new SocketFactory();
        }

        public IConnection Create()
        {
            return new Connection(IPAddress.Parse(_configuration.Address), _configuration.Port, _socketFactory);
        }
    }
}