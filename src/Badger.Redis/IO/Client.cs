using Badger.Redis.DataTypes;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal class Client : IClient
    {
        private readonly Reader _reader;
        private readonly Writer _writer;
        private readonly TcpClient _socket;

        public Client(TcpClient socket)
        {
            _socket = socket;
            _reader = new Reader(socket.GetStream());
            _writer = new Writer(socket.GetStream());
        }

        public void Dispose()
        {
            _socket.Close();
            _reader.Dispose();
            _writer.Dispose();
        }

        public async Task<IDataType> SendAsync(IDataType data, CancellationToken cancellationToken)
        {
            await _writer.WriteAsync(data, cancellationToken);
            return await _reader.ReadAsync(cancellationToken);
        }
    }
}