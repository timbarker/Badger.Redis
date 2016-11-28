using Badger.Redis.DataTypes;
using Badger.Redis.IO;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.Connection
{
    internal class BasicConnection : IOpenableConnection
    {
        private ISocketFactory _socketFactory;

        private interface IState<TState>
        {
            TState State { get; }
        }

        private class Disconnected : IState<ConnectionState>
        {
            public IPEndPoint EndPoint { get; set; }
            public ConnectionState State => ConnectionState.Disconnected;
        }

        private class Connected : IState<ConnectionState>
        {
            public ISocket Socket { get; set; }
            public ConnectionState State => ConnectionState.Connected;
        }

        private class Closed : IState<ConnectionState>
        {
            public ConnectionState State => ConnectionState.Closed;
        }

        private IState<ConnectionState> _state;
        public ConnectionState State => _state.State;

        public BasicConnection(IPEndPoint endPoint, ISocketFactory socketFactory)
        {
            _state = new Disconnected { EndPoint = endPoint };
            _socketFactory = socketFactory;
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            var state = GetState<Disconnected>();

            var socket = _socketFactory.Create(state.EndPoint);
            await socket.OpenAsync();
            _state = new Connected { Socket = socket };

            await PingAsync(cancellationToken);
        }

        private async Task<T> SendAsync<T>(IDataType request, CancellationToken cancellationToken) where T : IDataType
        {
            var state = GetState<Connected>();

            var response = await state.Socket.SendAsync(request, cancellationToken);
            if (response is T)
                return (T)response;

            throw new ConnectionException($"Invalid Response type - expected '{typeof(T).Name}' but got '{response.GetType().Name}'");
        }

        public async Task<string> PingAsync(CancellationToken cancellationToken)
        {
            GetState<Connected>();

            var response = await SendAsync<String>(new CommandBuilder().WithCommand(Command.PING).Build(), cancellationToken);
            if (response.Value != Response.PONG)
                throw new ConnectionException($"Invalid PING response - expected '{Response.PONG}' but got '{response}'");
            return response.Value;
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            var state = GetState<Connected>();

            await SendAsync<String>(new CommandBuilder().WithCommand(Command.QUIT).Build(), cancellationToken);
            state.Socket.Close();
            _state = new Closed();
        }

        private T GetState<T>() where T : class, IState<ConnectionState>
        {
            var state = _state as T;
            if (state == null)
                throw new ConnectionException($"Invalid Connection State - Required '{typeof(T).Name}' but was '{_state.GetType().Name}'");

            return state;
        }

        public void Dispose()
        {
            if (!(_state is Connected))
                return;

            var state = GetState<Connected>();
            state.Socket.Close();
            _state = new Closed();
        }
    }
}