# Badger.Redis

## Still in early development so don't use for anything that matters

### A simple .NET client for Redis
- Pooled connections
- Asynchronous programming interface

### Current functionality
- Establish connections to a single Redis server
- Send PING requests

### Basics

1. Create a connection pool on application startup
2. Get a connection from the pool
3. Use the connection
4. Return the connection to the pool
5. Dispose the connection pool on application shutdown

```cs
var config = new Configuration { Host = "127.0.0.1", Port = 6379 };

using (var pool = new ConnectionPool(config))
using (var connection = await pool.GetConnectionAsync())
{
    await connection.PingAsync(CancellationToken.None);
}
```

