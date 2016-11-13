using Badger.Redis.DataTypes;
using Badger.Redis.IO;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.Tests.IO
{
    public static class WriterTestExtensions
    {
        public static async Task<string> WriteStringAsync<T>(this IWriter<T> writer, T value) where T : IDataType
        {
            using (var ms = new MemoryStream())
            {
                await writer.WriteAsync(value, ms, CancellationToken.None);

                return Encoding.ASCII.GetString(ms.ToArray());
            }
        }
    }
}
