using Abstractions.IntermediateCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace IntermediateCache
{
    /// <summary>
    /// Intermediate cache client class
    /// </summary>
    public class IntermediateCacheClient : IIntermediateCacheClient
    {
        private readonly IOptions<IIntermediateCacheOptions> _options;
        private readonly IServiceProvider _serviceProvider;

        private readonly string _host;
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public IntermediateCacheClient(IOptions<IntermediateCacheOptions> options, IServiceProvider serviceProvider)
        {
            _options = options;
            _serviceProvider = serviceProvider;
            _host = options.Value.Host;

            using var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IHostedService>>();

            // Connection timeout counter
            int connectionTimoutCounter = 0;
            while (connectionTimoutCounter < 300)
            {
                try
                {
                    _connectionMultiplexer = ConnectionMultiplexer.Connect(_host);
                    if (_connectionMultiplexer.IsConnected == false)
                    {
                        connectionTimoutCounter++;
                        logger.LogError($"Failed to connect to Cache server. Attempt {connectionTimoutCounter} out of 300");
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
                    logger.LogError($"Failed to connect to Cache server. Attempt {connectionTimoutCounter} out of 300");
                    Task.Delay(1000).Wait();
                }
            }

            if (_connectionMultiplexer.IsConnected == false)
            {
                logger.LogError("Failure to connect to Cache server. Throwing exception");
                throw new Exception("Could not connect to Cache server");
            }
        }

        /// <summary>
        /// Increment a key in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> IncrementAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();

            return await db.StringIncrementAsync(key);
        }
    }
}
