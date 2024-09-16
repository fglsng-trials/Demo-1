using Abstractions.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MessageBroker
{
    /// <summary>
    /// Message broker client: Responsible for sending and receiving messages, using RabbitMQ
    /// </summary>
    public class MessageBrokerClient : IMessageBrokerClient
    {
        private readonly IOptions<IMessageBrokerOptions> _options;
        private readonly IServiceProvider _serviceProvider;

        private ConnectionFactory _factory;
        private string _queue;
        private IMessageConverter _messageConverter = new MessageConverter();

        public MessageBrokerClient(IOptions<MessageBrokerOptions> options, IServiceProvider serviceProvider)
        {
            _options = options;

            _factory = new ConnectionFactory { HostName = options.Value.Host };
            _queue = options.Value.Queue;
            _serviceProvider = serviceProvider;

            using var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IHostedService>>();

            // Connection timeout counter
            int connectionTimoutCounter = 0;
            while (connectionTimoutCounter < 300)
            {
                try
                {
                    using var connection = _factory.CreateConnection();
                    using var channel = connection.CreateModel();
                    channel.QueueDeclare(queue: _queue,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

                    if (channel.IsOpen == false)
                    {
                        connectionTimoutCounter++;
                        logger.LogError($"Failed to connect to message broker. Attempt {connectionTimoutCounter} out of 300");
                        Task.Delay(1000).Wait();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    connectionTimoutCounter++;
                    logger.LogError($"Failed to connect to message broker. Attempt {connectionTimoutCounter} out of 300");
                    Task.Delay(1000).Wait();
                }
            }

            if (connectionTimoutCounter > 300)
            {
                logger.LogError("Failure to connect to the message broker. Throwing exception");
                throw new Exception("Could not connect to the message broker");
            }
        }

            public async Task<IMessage> QueueAsync(object body)
            {
                var msg = new Message(body);

                using var connection = _factory.CreateConnection();
                using var channel = connection.CreateModel();

                var bodyEncoded = Encoding.UTF8.GetBytes(msg.ToString());

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: _queue,
                                     basicProperties: basicProperties,
                                     body: bodyEncoded);

                return msg;
            }

            public async Task<IMessage?> TakeSingleAsync()
            {
                using var connection = _factory.CreateConnection();
                using var channel = connection.CreateModel();

                var data = channel.BasicGet(queue: _queue,
                                            autoAck: true);

                if (data is null)
                {
                    return default;
                }

                try
                {
                    var msg = Encoding.UTF8.GetString(data.Body.ToArray());
                    return _messageConverter.ToMessage(msg);
                }
                catch (NullReferenceException)
                {
                    return default;
                }
            }
        }
    }
