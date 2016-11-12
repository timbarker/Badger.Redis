using Badger.Redis.DataTypes;

namespace Badger.Redis.Serialization
{
    public interface ISerializer<T> where T : IDataType
    {
        string Serialize(T value);
    }    
}
