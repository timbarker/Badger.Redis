using Badger.Redis.DataTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal interface IReader : IDisposable
    {
        Task<IDataType> ReadAsync(CancellationToken cancellationToken);
    }
}