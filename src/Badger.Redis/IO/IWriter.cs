﻿using Badger.Redis.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Badger.Redis.IO
{
    internal interface IWriter : IDisposable
    {
        Task WriteAsync(IRedisType value, CancellationToken cancellationToken);
    }
}