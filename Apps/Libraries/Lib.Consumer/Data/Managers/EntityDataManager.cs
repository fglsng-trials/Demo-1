using Abstractions.DataStorage;
using Lib.Consumer.Data.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Consumer.Data.Managers
{
    public class EntityDataManager : IDataStorageManager<Entity>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IHostedService> _logger;

        /// <summary>
        /// Constructor for the EntityDataManager class
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public EntityDataManager(IServiceProvider serviceProvider, ILogger<IHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Inserts an entity object into the database
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(Entity obj)
        {
            using var scope = _serviceProvider.CreateScope();
            using (var context = scope.ServiceProvider.GetService<DataContext>())
            {
                try
                {
                    context.Entities.Add(obj);
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"Entity with id {obj.Id} inserted");
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Failed to insert entity with id {obj.Id}");
                    return false;
                }
            }
        }
    }
}