using Lib.Shared;
using System.Globalization;

namespace Test.Consumer.BusinessLogic
{
    [TestClass]
    public class Test_Requirement1
    {
        /// <summary>
        /// Will test the Requirement1 method with a valid message body, where the timestamp is more than 1 minute old.
        /// </summary>
        [TestMethod]
        public void Test_Requirement1_Valid()
        {
            // Arrange
            var messageBody = new MessageBody
            {
                Counter = 0,
                Timestamp = (DateTime.UtcNow - TimeSpan.FromMinutes(2)) // 2 minutes ago
            };

            // Act
            var result = Lib.Consumer.BusinessLogic.RuleApplication.Requirement1(messageBody);

            // Assert
            Assert.IsTrue(result);
        }


        /// <summary>
        /// Will test the Requirement1 method with an invalid message body, where the timestamp is not 1 minute old.
        /// </summary>
        [TestMethod]
        public void Test_Requirement1_Invalid()
        {
            // Arrange
            var messageBody = new MessageBody
            {
                Counter = 0,
                Timestamp = DateTime.UtcNow
            };

            // Act
            var result = Lib.Consumer.BusinessLogic.RuleApplication.Requirement1(messageBody);

            // Assert
            Assert.IsFalse(result);
        }
    }
}