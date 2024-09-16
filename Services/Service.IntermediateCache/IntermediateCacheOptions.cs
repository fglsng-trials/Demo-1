using Abstractions.IntermediateCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateCache
{
    public record IntermediateCacheOptions : IIntermediateCacheOptions
    {
        public string? Host { get; set; }
    }
}
