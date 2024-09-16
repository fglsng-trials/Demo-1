using Abstractions.MessageBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageBroker
{
    internal class MessageConverter : IMessageConverter
    {
        public IMessage? ToMessage(string serializedMessage)
        {
            return JsonSerializer.Deserialize<Message>(serializedMessage);
        }
    }
}
