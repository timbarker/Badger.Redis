using System;

namespace Badger.Redis
{
    public class ConnectionPoolException : Exception
    {
        public ConnectionPoolException(string message)
            : base(message)
        {
        }
    }
}