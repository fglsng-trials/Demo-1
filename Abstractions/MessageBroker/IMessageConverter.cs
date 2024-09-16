using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.MessageBroker
{
    public interface IMessageConverter
    {
        IMessage? ToMessage(string serializedMessage);
    }
}
