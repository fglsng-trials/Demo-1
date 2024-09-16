using Lib.Consumer.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Test.Consumer")]
namespace Lib.Consumer.Data
{
    /// <summary>
    /// Class for handling the database context
    /// </summary>
    public class DataContext : DbContext, IDataContext
    {
        private static bool _isMigrated; // Flag to check if the database has been migrated
        private readonly IServiceProvider _serviceProvider;

        public DbSet<Entity> Entities { get; set; }


        public DataContext(DbContextOptions<DataContext> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;
          
            using var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IHostedService>>();

            // Connection timeout counter
            int connectionTimoutCounter = 0;
            while (!_isMigrated && connectionTimoutCounter < 300)
            {
                try
                {
                    // Check if the database has been migrated, if not, migrate it
                    Database.Migrate();
                    _isMigrated = true;
                    break;
                }
                catch (Exception e)
                {
                    logger.LogError($"Failed to connect to database. Attempt {connectionTimoutCounter} out of 300");
                    connectionTimoutCounter++;
                    Task.Delay(1000).Wait();
                }
            }

            if (connectionTimoutCounter > 300)
            {
                logger.LogError("Failure to connect to the database. Throwing exception");
                throw new Exception("Could not connect to the database");
            }

            
        }
    }
}
