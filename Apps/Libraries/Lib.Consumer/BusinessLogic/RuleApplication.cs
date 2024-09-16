using Abstractions.IntermediateCache;
using Abstractions.MessageBroker;
using Lib.Shared;
using Lib.Consumer.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Abstractions.DataStorage;

[assembly: InternalsVisibleTo("Test.Consumer")]
namespace Lib.Consumer.BusinessLogic
{
    /// <summary>
    /// Class for applying business rules to a message.
    /// </summary>
    public class RuleApplication : IRuleApplication
    {
        private readonly ILogger<IHostedService> _logger;
        private readonly IDataStorageManager<Entity> _dbEntityManager;
        private readonly IServiceProvider _serviceProvider;

        public RuleApplication(ILogger<IHostedService> logger, IDataStorageManager<Entity> dbEntityManager, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _dbEntityManager = dbEntityManager;
        }

        /// <summary>
        /// Applying required business logic to a message.
        /// </summary>
        /// <param name="body">Message body</param>
        /// <returns></returns>
        public async Task<IRuleApplicationResult> Apply(MessageBody body)
        {
            var result = new RuleApplicationResult();

            using var scope = _serviceProvider.CreateScope();
            var messageClient = scope.ServiceProvider.GetRequiredService<IMessageBrokerClient>();
            var cacheClient = scope.ServiceProvider.GetRequiredService<IIntermediateCacheClient>();

            if (Requirement1(body)) // Requirement 1: "Take message from queue. If the message timestamp is over 1 minute old - then discard it"
            {
                _logger.LogInformation($"Message is over 1 minute old and will be discarded");
                result.Success = true;
                result.RequirementMatch = 1;
                return result;
            }

            if (Requirement2(body)) // Requirement 2: "If the message is under 1 minute old and the second on the timestamp is an even number, then put the message in a database (azure db, postgres, mongo, other)"
            {
                _logger.LogInformation($"Message is under 1 minute old and the second ({body.Timestamp.Second}) on the timestamp is an even number");

                var created = await _dbEntityManager.InsertAsync(new Entity(body.Counter, body.Timestamp));

                if (created == false)
                {
                    _logger.LogError("Failed to insert into database. Re-queueing message.");
                    await messageClient.QueueAsync(body); // Re-queue message if failed to insert into database. This has the potential to create an infinite loop if the database is always down, although it's a valid mechanism as mssql server is slow to start up using docker compose.
                    result.Success = false;
                    result.RequirementMatch = 2;
                    return result;
                }

                _logger.LogInformation($"Inserted entry into database");

                result.Success = true;
                result.RequirementMatch = 2;
                return result;
            }

            if (Requirement3(body)) // Requirement 3: "If the message is under 1 minute old and the second on the timestamp is an odd number, then put the message back in the queue with a counter increment of +1"
            {
                _logger.LogInformation($"Message is under 1 minute old and the second ({body.Timestamp.Second}) on the timestamp is an odd number");
                var increment = await cacheClient.IncrementAsync("counter");

                if (increment <= body.Counter)
                {
                    result.Success = false;
                    result.RequirementMatch = 3;
                    _logger.LogError("Failed to increment counter.");
                    return result;
                }

                body.Counter = increment;
                await messageClient.QueueAsync(body);
                result.Success = true;
                result.RequirementMatch = 3;
                return result;
            }

            return result;
        }

        /// <summary>
        /// Requirement 1: "Take message from queue. If the message timestamp is over 1 minute old - then discard it"
        /// </summary>
        /// <param name="body">Message body</param>
        /// <returns></returns>
        internal static bool Requirement1(MessageBody body)
        {
            return body.Timestamp < DateTime.UtcNow.AddMinutes(-1);
        }

        /// <summary>
        /// Requirement 2: "If the message is under 1 minute old and the second on the timestamp is an even number, then put the message in a database (azure db, postgres, mongo, other)"
        /// </summary>
        /// <param name="body">Message body</param>
        /// <returns></returns>
        internal static bool Requirement2(MessageBody body)
        {
            return IsEven(body.Timestamp.Second);
        }

        /// <summary>
        /// Requirement 3: "If the message is under 1 minute old and the second on the timestamp is an odd number, then put the message back in the queue with a counter increment of +1"
        /// </summary>
        /// <param name="body">Message body</param>
        /// <returns></returns>
        internal static bool Requirement3(MessageBody body)
        {
            return !IsEven(body.Timestamp.Second);
        }

        /// <summary>
        /// Determines if a number is even.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        internal static bool IsEven(int number)
        {
            return number % 2 == 0;
        }

    }
}
