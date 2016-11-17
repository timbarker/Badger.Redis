using Badger.Redis.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Badger.Redis.Tests.IO
{
    public class ConnectionTest
    {
        [Fact]
        public async Task Connect()
        {
            var connection = new Connection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379));
            await connection.Open();
            await connection.Close();
        }
    }
}
