﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.IntermediateCache
{
    public interface IIntermediateCacheClient
    {
        Task<long> IncrementAsync(string key);
    }
}
