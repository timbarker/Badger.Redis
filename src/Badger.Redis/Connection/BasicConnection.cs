using Badger.Redis.Commands;
using Badger.Redis.IO;
using Badger.Redis.Types;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.Connection
{
    internal class BasicConnection : IOpenableConnection
    {
        private IClientFactory _clientFactory;

        private interface IState : IDisposable
        {
            ConnectionState State { get; }
        }

        private class Disconnected : IState
        {
            public IPEndPoint EndPoint { get; set; }
            public ConnectionState State => ConnectionState.Disconnected;

            public void Dispose()
            {
            }
        }

        private class Connected : IState
        {
            public IClient Client { get; set; }
            public ConnectionState State => ConnectionState.Connected;

            public void Dispose()
            {
                Client.Dispose();
            }
        }

        private class Closed : IState
        {
            public ConnectionState State => ConnectionState.Closed;

            public void Dispose()
            {
            }
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

        private async Task<T> SendAsync<T>(IRedisType request, CancellationToken cancellationToken) where T : IRedisType
        {
            var state = GetState<Connected>();

            IRedisType response;
            try
            {
                response = await state.Client.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Close();
                throw new ConnectionException("Error while making request", ex);
            }

            if (response is T)
                return (T)response;

            throw new ConnectionException($"Invalid Response type - expected '{typeof(T).Name}' but got '{response.GetType().Name}'");
        }

        public async Task<string> PingAsync(CancellationToken cancellationToken)
        {
            GetState<Connected>();

            var response = await SendAsync<RedisString>(new CommandBuilder().WithCommand(Command.PING).Build(), cancellationToken);
            if (response.Value != Response.PONG)
                throw new ConnectionException($"Invalid PING response - expected '{Response.PONG}' but got '{response}'");
            return response.Value;
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            var state = GetState<Connected>();

            await SendAsync<RedisString>(new CommandBuilder().WithCommand(Command.QUIT).Build(), cancellationToken);
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

        private void Close()
        {
            _state.Dispose();
            _state = new Closed();
        }

        public void Dispose()
        {
            Close();
        }
    }
}