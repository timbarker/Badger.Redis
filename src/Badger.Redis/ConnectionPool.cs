using Badger.Redis.Connection;
using Badger.Redis.IO;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis
{
    public class ConnectionPool : IConnectionPool
    {
        private class PooledConnection : IConnection
        {
            private readonly ConnectionPool _pool;
            private readonly IConnection _connection;
            private bool disposed;

            public PooledConnection(IConnection connection, ConnectionPool pool)
            {
                _connection = connection;
                _pool = pool;
            }

            public ConnectionState State => PassThrough(c => c.State);

            public Task<string> PingAsync(CancellationToken cancellationToken) => PassThrough(c => c.PingAsync(cancellationToken));

            private T PassThrough<T>(Func<IConnection, T> func)
            {
                if (disposed) throw new ObjectDisposedException(nameof(IConnection));

                return func(_connection);
            }

            public void Dispose()
            {
                if (disposed) return;

                disposed = true;
                _pool.Return(_connection);
            }
        }

        private readonly Configuration _configuration;
        private readonly IConnectionCreator _connectionCreator;
        private readonly ConcurrentQueue<IConnection> _availableConnections;
        private int _activeConnections;
        private bool _disposed;

        public ConnectionPool(Configuration configuration)
            : this(configuration, new ConnectionCreator(new BasicConnectionFactory(new ClientFactory())))
        {
        }

        internal ConnectionPool(Configuration configuration, IConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
            _configuration = configuration;
            _availableConnections = new ConcurrentQueue<IConnection>();
        }

        public int ActiveConnections => _activeConnections;

        public Task<IConnection> GetConnectionAsync()
        {
            return GetConnectionAsync(CancellationToken.None);
        }

        public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ConnectionPool));

            if (Interlocked.Increment(ref _activeConnections) > _configuration.MaxPoolSize)
            {
                Interlocked.Decrement(ref _activeConnections);
                throw new ConnectionPoolException($"Connection Pool size exceeded - MaxPoolSize: {_configuration.MaxPoolSize}");
            }

            try
            {
                return new PooledConnection(await GetPooledOrOpenConnectionAsync(cancellationToken), this);
            }
            catch (Exception)
            {
                Interlocked.Decrement(ref _activeConnections);
                throw;
            }
        }

        private async Task<IConnection> GetPooledOrOpenConnectionAsync(CancellationToken cancellationToken)
        {
            var conn = await GetPooledConnectionAsync(cancellationToken);
            if (conn != null) return conn;

            return await _connectionCreator.CreateOpenedAsync(_configuration.Host, _configuration.Port, cancellationToken);
        }

        private async Task<IConnection> GetPooledConnectionAsync(CancellationToken cancellationToken)
        {
            IConnection conn = null;
            while (_availableConnections.TryDequeue(out conn))
            {
                try
                {
                    await conn.PingAsync(cancellationToken);
                    return conn;
                }
                catch (Exception)
                {
                    conn.Dispose();
                }
            }

            return null;
        }

        private void Return(IConnection conn)
        {
            if (!_disposed && conn.State == ConnectionState.Connected)
            {
                _availableConnections.Enqueue(conn);
            }
            else
            {
                conn.Dispose();
            }

            Interlocked.Decrement(ref _activeConnections);
        }

        public void Dispose()
        {
            _disposed = true;

            IConnection conn;
            while (_availableConnections.TryDequeue(out conn))
            {
                conn.Dispose();
            }
        }
    }
}