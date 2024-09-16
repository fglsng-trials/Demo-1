using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.MessageBroker
{
    public interface IMessageBrokerClient
    {
        Task<IMessage> TakeSingleAsync();
        Task<IMessage> QueueAsync(object body);

    }
}
