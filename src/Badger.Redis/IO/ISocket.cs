using Badger.Redis.DataTypes;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    public interface ISocket
    {
        Task OpenAsync();

        void Close();

        Task<IDataType> SendAsync(IDataType request, CancellationToken cancellationToken);
    }
}