using Badger.Redis.DataTypes;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    public interface IWriter<T> where T : IDataType
    {
        Task WriteAsync(T value, Stream stream, CancellationToken cancellationToken);
    }    
}
