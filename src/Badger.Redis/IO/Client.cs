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
        private readonly SemaphoreSlim _sem;

        public Client(TcpClient socket)
        {
            _socket = socket;
            _reader = new Reader(socket.GetStream());
            _writer = new Writer(socket.GetStream());
            _sem = new SemaphoreSlim(1, 1);
        }

        public void Dispose()
        {
            _socket.Close();
            _reader.Dispose();
            _writer.Dispose();
            _sem.Dispose();
        }

        public async Task<IDataType> SendAsync(IDataType data, CancellationToken cancellationToken)
        {
            await _sem.WaitAsync(cancellationToken);

            try
            {
                await _writer.WriteAsync(data, cancellationToken);
                return await _reader.ReadAsync(cancellationToken);
            }
            finally
            {
                _sem.Release();
            }
        }
    }
}