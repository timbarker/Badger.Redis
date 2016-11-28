using Badger.Redis.DataTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal interface IClient : IDisposable
    {
        Task<IDataType> SendAsync(IDataType request, CancellationToken cancellationToken);
    }
}