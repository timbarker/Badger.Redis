namespace Badger.Redis
{
    public class Configuration
    {
        public string Address { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
        public int MaxPoolSize { get; set; } = 10;
    }
}