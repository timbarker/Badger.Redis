using System.Text;

namespace Badger.Redis.DataTypes
{
    public class Key : BulkString
    {
        public Key(params object[] parts)
            : this(string.Join(":", parts))
        { }

        public Key(char seperator, params object[] parts)
            : this(string.Join(seperator.ToString(), parts))
        { }

        public Key(string key)
            : base(Encoding.UTF8.GetBytes(key))
        { }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Value);
        }
    }
}
