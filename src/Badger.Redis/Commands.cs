namespace Badger.Redis
{
    internal static class Commands
    {
        public const string PING = "PING";
        public const string QUIT = "QUIT";
    }

    internal static class Responses
    {
        public const string PONG = "PONG";
        public const string OK = "OK";
    }
}