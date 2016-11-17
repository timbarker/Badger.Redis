using Badger.Redis.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Array = Badger.Redis.DataTypes.Array;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.IO
{
    static class Commands
    {
        public const string PING = "PING";
        public const string QUIT = "QUIT";
    }

    class IConnectionState
    { }

    class Disconnected : IConnectionState
    { }

    class Connected : IConnectionState
    {
        public IWriter Writer { get; }
        public IReader Reader { get; }
        public TcpClient Socket { get; }

        public Connected(TcpClient socket, IWriter writer, IReader reader)
        {
            Socket = socket;
            Writer = writer;
            Reader = reader;
        }
    }

    public class Connection
    {
        private IPEndPoint _endPoint;

        private IConnectionState _state;

        public Connection(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _state = new Disconnected();
        }

        public async Task Open()
        {
            if (!IsState<Disconnected>()) return;

            var socket = new TcpClient();
            await socket.ConnectAsync(_endPoint.Address, _endPoint.Port);
            var writer = new Writer(socket.GetStream());
            var reader = new Reader(socket.GetStream());

            _state = new Connected(socket, writer, reader);

            var resp = await SendCommand(Commands.PING, BulkString.FromString("Hello World"));
        }

        public async Task Close()
        {
            var connection = GetState<Connected>();

            var resp = await SendCommand(Commands.QUIT);
            connection.Socket.Close();
            _state = new Disconnected();
        }

        private bool TryGetState<T>(out T state) where T : IConnectionState
        {
            state = _state as T;
            return state != null;
        }

        private T GetState<T>() where T : IConnectionState
        {
            T state = null;
            if (!TryGetState(out state))
                throw new InvalidOperationException($"Invalid State - Required '{typeof(T).Name}' but was '{_state.GetType().Name}'");

            return state;
        }

        private bool IsState<T>() where T : IConnectionState
        {
            return _state is T;
        }

        private async Task<IDataType> Send(IDataType data)
        {
            var connection = GetState<Connected>();

            await connection.Writer.WriteAsync(data, CancellationToken.None);
            return await connection.Reader.ReadAsync(CancellationToken.None);
        }

        private async Task<IDataType> SendCommand(string command, params IDataType[] args)
        {
            return await Send(new Array(new[] { BulkString.FromString(command) }.Concat(args).ToArray()));
        }
    }
}
