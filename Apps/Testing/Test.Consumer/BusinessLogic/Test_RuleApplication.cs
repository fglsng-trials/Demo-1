using Abstractions.DataStorage;
using Abstractions.IntermediateCache;
using Abstractions.MessageBroker;
using IntermediateCache;
using Lib.Consumer.BusinessLogic;
using Lib.Consumer.Data;
using Lib.Consumer.Data.Model;
using Lib.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Consumer.BusinessLogic
{
    [TestClass]
    public class Test_RuleApplication
    {
        private readonly Mock<IIntermediateCacheClient> _intermediateCacheClient;
        private readonly Mock<IDataStorageManager<Entity>> _storageManager;
        private readonly Mock<IMessageBrokerClient> _messageBrokerClient;
        private readonly Mock<IDataContext> _context;
        private readonly Mock<IServiceProvider> _serviceProvider;
        private readonly Mock<ILogger<IHostedService>> _logger;

        public Test_RuleApplication()
        {
            _intermediateCacheClient = new Mock<IIntermediateCacheClient>();
            _storageManager = new Mock<IDataStorageManager<Entity>>();
            _messageBrokerClient = new Mock<IMessageBrokerClient>();
            _context = new Mock<IDataContext>();
            _serviceProvider = new Mock<IServiceProvider>();
            _logger = new Mock<ILogger<IHostedService>>();
        }

        /// <summary>
        /// Tests if Requirement 1 is activated correctly.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Apply_WhenCalled_Requirement1()
        {
            // Arrange
            var messageBody = new MessageBody() { Counter = 1, Timestamp = DateTime.ParseExact("1800-01-01 00:00:01", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)};

            ArrangeServiceProvider();

            var ruleApplication = new RuleApplication(_logger.Object, _storageManager.Object, _serviceProvider.Object);

            // Act
            var result = await ruleApplication.Apply(messageBody);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.RequirementMatch);
        }

        /// <summary>
        /// Tests if Requirement 2 is activated corretly and the message is inserted in the database.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Apply_WhenCalled_Requirement2_InsertedInDb()
        {
            // Arrange
            var messageBody = new MessageBody() { Counter = 1, Timestamp = DateTime.ParseExact((DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:02"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) };

            _storageManager.Setup(x => x.InsertAsync(It.IsAny<Entity>())).ReturnsAsync(true);
            ArrangeServiceProvider();

            var ruleApplication = new RuleApplication(_logger.Object, _storageManager.Object, _serviceProvider.Object);

            // Act
            var result = await ruleApplication.Apply(messageBody);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.RequirementMatch);
        }

        /// <summary>
        /// Tests if Requirement 2 is activated corretly and the message is not inserted in the database, but requeued instead.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Apply_WhenCalled_Requirement2_Requeued()
        {
            // Arrange
            var messageBody = new MessageBody() { Counter = 1, Timestamp = DateTime.ParseExact((DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:02"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) };

            _storageManager.Setup(x => x.InsertAsync(It.IsAny<Entity>())).ReturnsAsync(false);
            _intermediateCacheClient.Setup(x => x.IncrementAsync(It.IsAny<string>())).ReturnsAsync(1);
            ArrangeServiceProvider();

            var ruleApplication = new RuleApplication(_logger.Object, _storageManager.Object, _serviceProvider.Object);

            // Act
            var result = await ruleApplication.Apply(messageBody);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(2, result.RequirementMatch);
        }

        /// <summary>
        /// Tests if Requirement 3 is activated corretly and the message is requeued with an incremented counter.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Apply_WhenCalled_Requirement3_Valid_Increment()
        {
            // Arrange
            var messageBody = new MessageBody() { Counter = 1, Timestamp = DateTime.ParseExact((DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:01"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) };

            _intermediateCacheClient.Setup(x => x.IncrementAsync(It.IsAny<string>())).ReturnsAsync(2);
            ArrangeServiceProvider();

            var ruleApplication = new RuleApplication(_logger.Object, _storageManager.Object, _serviceProvider.Object);

            // Act
            var result = await ruleApplication.Apply(messageBody);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.RequirementMatch);
        }

        /// <summary>
        /// Tests if Requirement 3 is activated corretly and the message is not requeued because of failed incremental.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Apply_WhenCalled_Requirement3_Invalid_Increment()
        {
            // Arrange
            var messageBody = new MessageBody() { Counter = 1, Timestamp = DateTime.ParseExact((DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:01"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) };

            _intermediateCacheClient.Setup(x => x.IncrementAsync(It.IsAny<string>())).ReturnsAsync(1);
            ArrangeServiceProvider();

            var ruleApplication = new RuleApplication(_logger.Object, _storageManager.Object, _serviceProvider.Object);

            // Act
            var result = await ruleApplication.Apply(messageBody);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(3, result.RequirementMatch);
        }

        private void ArrangeServiceProvider()
        {
            _serviceProvider.Setup(x => x.GetService(typeof(IIntermediateCacheClient))).Returns(_intermediateCacheClient.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IDataStorageManager<Entity>))).Returns(_storageManager.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(ILogger<IHostedService>))).Returns(_logger.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IMessageBrokerClient))).Returns(_messageBrokerClient.Object);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            _serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

        }
        
    }
}
