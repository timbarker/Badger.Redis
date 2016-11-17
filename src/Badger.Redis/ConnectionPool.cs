using Badger.Redis.IO;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;


namespace Badger.Redis
{

    public class ConnectionPool : IDisposable
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
                    throw new ObjectDisposedException("Connection");
                
                return _connection.PingAsync(cancellationToken);
            }

            public void Dispose()
            {
                disposed = false;
                _pool.ReturnToPool(_connection);
            }
        }

        private ISocketFactory _socketFactory;
        private Configuration _configuration;
        private Queue<Connection> _availableConnections;
        private List<Connection> _activeConnections;

        public ConnectionPool(Configuration configuration)
        {
            _configuration = configuration;
            _socketFactory = new SocketFactory();
            _availableConnections = new Queue<Connection>(configuration.MaxPoolSize);
            _activeConnections = new List<Connection>();
        }

        public async Task<IConnection> ConnectAsync(CancellationToken cancellationToken)
        {
            Connection conn = null;
            if (_availableConnections.Count > 0)
            {
                conn = _availableConnections.Dequeue();
                await conn.PingAsync(cancellationToken);
                // todo what to do if ping fails?
            }
            else if (_activeConnections.Count < _configuration.MaxPoolSize)
            {
                conn = new Connection(IPAddress.Parse(_configuration.Address), _configuration.Port, _socketFactory);

                await conn.ConnectAsync(cancellationToken);
            }
            else
            {
                throw new Exception("ConnectionPool size exceeded");
            }

            _activeConnections.Add(conn);

            return conn;
        }

        private void ReturnToPool(Connection conn)
        {
            _activeConnections.Remove(conn);
            _availableConnections.Enqueue(conn);
        }

        public void Dispose()
        {
            foreach (var conn in _availableConnections)
            {
                conn.Dispose();
            }

            foreach (var conn in _activeConnections)
            {
                conn.Dispose();
            }
        }
    }
}