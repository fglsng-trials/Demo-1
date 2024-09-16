using Abstractions.IntermediateCache;
using Abstractions.MessageBroker;
using Lib.Shared;

namespace App.Producer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var cacheClient = scope.ServiceProvider.GetRequiredService<IIntermediateCacheClient>();
            var messageClient = scope.ServiceProvider.GetRequiredService<IMessageBrokerClient>();

            

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var body = new MessageBody() { Counter = await cacheClient.IncrementAsync("counter"), Timestamp = DateTime.UtcNow };
                var msg = await messageClient.QueueAsync(body);

                if (msg is null)
                {
                    _logger.LogError("Failure to send message");
                }
                else
                {
                    _logger.LogInformation($"Queried message {msg.Id} | Created (UTC) {msg.Created?.ToString("G")}");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
