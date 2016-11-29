using Badger.Redis.Commands;
using Badger.Redis.DataTypes;
using Badger.Redis.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis.Connection
{
    internal class BasicConnection : IOpenableConnection
    {
        private IClientFactory _clientFactory;

        private interface IState
        {
            ConnectionState State { get; }
        }

        private class Disconnected : IState
        {
            public IPEndPoint EndPoint { get; set; }
            public ConnectionState State => ConnectionState.Disconnected;
        }

        private class Connected : IState
        {
            public IClient Client { get; set; }
            public ConnectionState State => ConnectionState.Connected;
        }

        private class Closed : IState
        {
            public ConnectionState State => ConnectionState.Closed;
        }

        private IState _state;
        public ConnectionState State => _state.State;

        public BasicConnection(IPEndPoint endPoint, IClientFactory clientFactory)
        {
            _state = new Disconnected { EndPoint = endPoint };
            _clientFactory = clientFactory;
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            var state = GetState<Disconnected>();

            var socket = await _clientFactory.CreateConnectedAsync(state.EndPoint);
            _state = new Connected { Client = socket };

            await PingAsync(cancellationToken);
        }

        private async Task<T> SendAsync<T>(IDataType request, CancellationToken cancellationToken) where T : IDataType
        {
            var state = GetState<Connected>();

            var response = await state.Client.SendAsync(request, cancellationToken);
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
            state.Client.Dispose();
            _state = new Closed();
        }

        private T GetState<T>() where T : class, IState
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
            state.Client.Dispose();
            _state = new Closed();
        }
    }
}