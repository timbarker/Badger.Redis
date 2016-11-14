using Badger.Redis.DataTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    public interface IWriter: IDisposable
    {
        Task WriteAsync(IDataType value, CancellationToken cancellationToken);
    }    
}
