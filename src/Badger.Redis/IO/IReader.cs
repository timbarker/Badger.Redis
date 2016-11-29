using Badger.Redis.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal interface IReader : IDisposable
    {
        Task<IRedisType> ReadAsync(CancellationToken cancellationToken);
    }
}