using Badger.Redis.IO;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Concurrent;

namespace Badger.Redis
{
    public class ConnectionPool : IConnectionPool
    {
        private class PooledConnection : IConnection
        {
            private ConnectionPool _pool;
            private Connection _connection;
            private bool disposed;

            public PooledConnection(Connection connection, ConnectionPool pool)
            {
                _connection = connection;
                _pool = pool;
            }

            public Task<string> PingAsync(CancellationToken cancellationToken)
            {
                if (disposed)
                    throw new ObjectDisposedException(nameof(IConnection));
                
                return _connection.PingAsync(cancellationToken);
            }

            public void Dispose()
            {
                if (disposed) return;

                disposed = true;
                _pool.Return(_connection);
            }
        }

        private readonly ISocketFactory _socketFactory;
        private readonly Configuration _configuration;
        private readonly ConcurrentQueue<Connection> _availableConnections;
        private int _activeConnections;
        private bool _disposed;

        public ConnectionPool(Configuration configuration)
        {
            _configuration = configuration;
            _socketFactory = new SocketFactory();
            _availableConnections = new ConcurrentQueue<Connection>();
        }

        public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ConnectionPool));
            
            var conn = await GetOrCreateConnectionAsync(cancellationToken);

            return conn;
        }

        private async Task<Connection> GetOrCreateConnectionAsync(CancellationToken cancellationToken)
        {
            var conn = await GetPooledConnectionAsync(cancellationToken);
            if (conn != null) return conn;

            if (Interlocked.Increment(ref _activeConnections) <= _configuration.MaxPoolSize)
            {
                try
                {
                    conn = new Connection(IPAddress.Parse(_configuration.Address), _configuration.Port, _socketFactory);
                    await conn.ConnectAsync(cancellationToken);
                    return conn;
                }
                catch (Exception)
                {
                    Interlocked.Decrement(ref _activeConnections);
                    throw;
                }
            }

            throw new Exception($"ConnectionPool size exceeded - MaxPoolSize: {_configuration.MaxPoolSize}");
        }

        private async Task<Connection> GetPooledConnectionAsync(CancellationToken cancellationToken)
        {
            Connection conn = null;
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

        private void Return(Connection conn)
        {
            if (_disposed)
            {
                conn.Dispose();
                return;
            }

            _availableConnections.Enqueue(conn);

            Interlocked.Decrement(ref _activeConnections);
        }

        public void Dispose()
        {
            _disposed = true;

            Connection conn;
            while (_availableConnections.TryDequeue(out conn))
            {
                conn.Dispose();
            }
        }
    }
}