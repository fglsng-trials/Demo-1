using Abstractions.MessageBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker
{
    public record MessageBrokerOptions : IMessageBrokerOptions
    {
        public string? Host { get; set; }
        public string? Queue { get; set; }

    }
}
