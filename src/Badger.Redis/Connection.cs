using Badger.Redis.DataTypes;
using Badger.Redis.IO;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using String = Badger.Redis.DataTypes.String;

namespace Badger.Redis
{
    public class Connection : IConnection
    {
        private ISocketFactory _socketFactory;

        private interface IState
        { }

        private class Disconnected : IState
        {
            public IPEndPoint EndPoint { get; set; }
        }

        private class Connected : IState
        {
            public ISocket Socket { get; set; }
            public IPEndPoint EndPoint { get; set; }
        }

        private class Closed : IState
        { }

        private IState _state;

        public Connection(IPAddress address, int port, ISocketFactory socketFactory)
        {
            _state = new Disconnected { EndPoint = new IPEndPoint(address, port) };
            _socketFactory = socketFactory;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            var state = GetState<Disconnected>();

            var socket = _socketFactory.Create(state.EndPoint);
            await socket.OpenAsync();

            _state = new Connected { Socket = socket, EndPoint = state.EndPoint };

            var pong = await PingAsync(cancellationToken);
            if (pong != Responses.PONG)
                throw new Exception($"Invalid PING response - expected '{Responses.PONG}' but got '{pong}'");
        }

        private async Task<T> SendAsync<T>(IDataType request, CancellationToken cancellationToken) where T : IDataType
        {
            var state = GetState<Connected>();

            var response = await state.Socket.SendAsync(request, cancellationToken);
            if (response is T)
                return (T)response;

            throw new Exception($"Invalid Response type - expected '{typeof(T).Name}' but got '{response.GetType().Name}'");
        }

        public async Task<string> PingAsync(CancellationToken cancellationToken)
        {
            GetState<Connected>();

            var response = await SendAsync<String>(new CommandBuilder().WithCommand(Commands.PING).Build(), cancellationToken);

            return response.Value;

        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            var state = GetState<Connected>();

            await SendAsync<String>(new CommandBuilder().WithCommand(Commands.QUIT).Build(), cancellationToken);
            state.Socket.Close();

            _state = new Disconnected { EndPoint = state.EndPoint };
        }

        private T GetState<T>() where T : class, IState
        {
            var state = _state as T;
            if (state == null)
                throw new InvalidOperationException($"Invalid Connection State - Required '{typeof(T).Name}' but was '{_state.GetType().Name}'");

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