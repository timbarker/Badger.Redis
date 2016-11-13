using Badger.Redis.DataTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    public interface IWriter<T> : IDisposable where T : IDataType
    {
        Task WriteAsync(T value, CancellationToken cancellationToken);
    }    
}
