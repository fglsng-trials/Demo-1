using Abstractions.DataStorage;
using Abstractions.MessageBroker;
using Lib.Consumer.BusinessLogic;
using Lib.Consumer.Data.Managers;
using Lib.Consumer.Data.Model;
using Lib.Shared;

namespace App.Consumer
{

    /// <summary>
    /// Base Worker class that handles the message processing. Runs continuously and processes messages from the message broker.
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        private readonly IDataStorageManager<Entity> _dbEntityManager;

        private readonly IRuleApplication _ruleApplication;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _dbEntityManager = new EntityDataManager(_serviceProvider, _logger);

            _ruleApplication = new RuleApplication(_logger, _dbEntityManager, _serviceProvider);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var message = await GetNextMessage();

                if (message is not null)
                {
                    var messageBody = message.TryGetBody<MessageBody>();
                    if (messageBody is not null)
                    {
                        await _ruleApplication.Apply(messageBody);
                    }
                }


                // Delay for 100ms
                await Task.Delay(100, stoppingToken);
            }
        }

        private async Task<IMessage?> GetNextMessage()
        {
            using var scope = _serviceProvider.CreateScope();
            var messageClient = scope.ServiceProvider.GetRequiredService<IMessageBrokerClient>();

            var message = await messageClient.TakeSingleAsync();

            if (message is null)
            {
                _logger.LogInformation($"No messages recieved");
                return default;
            }

            _logger.LogInformation($"Recieved message {message.Id} | Created (UTC) {message.Created?.ToString("G")}");
            return message;
        }
    }
}
