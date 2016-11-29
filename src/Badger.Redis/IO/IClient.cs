using Badger.Redis.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal interface IClient : IDisposable
    {
        Task<IRedisType> SendAsync(IRedisType request, CancellationToken cancellationToken);
    }
}