using System;

namespace Badger.Redis
{
    public class ConnectionException : Exception
    {
        public ConnectionException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public ConnectionException(string message)
            : base(message)
        {
        }
    }
}