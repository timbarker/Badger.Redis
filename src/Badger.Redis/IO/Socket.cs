using Badger.Redis.DataTypes;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    public class Socket : ISocket
    {
        private interface IState
        { }

        private static class State
        {
            public static Disconnected Disconnected(IPEndPoint endPoint)
            {
                return new Disconnected { EndPoint = endPoint };
            }

            public static Connected Connected(TcpClient socket, IWriter writer, IReader reader)
            {
                return new Connected { Socket = socket, Writer = writer, Reader = reader };
            }

            public static Closed Closed()
            {
                return new Closed();
            }

            public static Failed Failed(Exception exception)
            {
                return new Failed { Exception = exception };
            }
        }

        private class Disconnected : IState
        {
            public IPEndPoint EndPoint { get; set; }
        }

        private class Connected : IState
        {
            public IWriter Writer { get; set; }
            public IReader Reader { get; set; }
            public TcpClient Socket { get; set; }
        }

        private class Closed : IState
        { }

        private class Failed : IState
        {
            public Exception Exception { get; set; }
        }

        private IState _state;

        public Socket(IPEndPoint endPoint)
        {
            _state = State.Disconnected(endPoint);
        }

        public async Task OpenAsync()
        {
            var disconected = GetState<Disconnected>();

            try
            {
                var socket = new TcpClient();
                await socket.ConnectAsync(disconected.EndPoint.Address, disconected.EndPoint.Port);
                var reader = new Reader(socket.GetStream());
                var writer = new Writer(socket.GetStream());

                _state = State.Connected(socket, writer, reader);
            }
            catch (Exception ex)
            {
                _state = State.Failed(ex);
                throw;
            }
        }

        public void Close()
        {
            var connection = GetState<Connected>();

            connection.Socket.Close();
            _state = State.Closed();
        }

        public async Task<IDataType> SendAsync(IDataType data, CancellationToken cancellationToken)
        {
            var connection = GetState<Connected>();

            try
            {
                await connection.Writer.WriteAsync(data, cancellationToken);
                return await connection.Reader.ReadAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _state = State.Failed(ex);
                throw;
            }
        }

        private T GetState<T>() where T : class, IState
        {
            var state = _state as T;
            if (state == null)
                throw new InvalidOperationException($"Invalid Socket State - Required '{typeof(T).Name}' but was '{_state.GetType().Name}'");

            return state;
        }
    }
}