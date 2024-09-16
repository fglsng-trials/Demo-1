using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.MessageBroker
{
    public interface IMessageBrokerOptions
    {
        string? Host { get; set; }
        string? Queue { get; set; }
    }
}
