using Lib.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Consumer.BusinessLogic
{
    [TestClass]
    public class Test_Requirement2
    {
        /// <summary>
        /// Will test the Requirement2 method with a valid message body, where the second of the timestamp is an even number.
        /// </summary>
        [TestMethod]
        public void Test_Requirement2_Valid()
        {
            // Arrange
            var messageBody = new MessageBody
            {
                Counter = 0,
                Timestamp = DateTime.ParseExact("2024-01-01 00:00:02", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            // Act
            var result = Lib.Consumer.BusinessLogic.RuleApplication.Requirement2(messageBody);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Will test the Requirement2 method with an invalid message body, where the second of the timestamp is an odd number.
        /// </summary>
        [TestMethod]
        public void Test_Requirement2_Invalid()
        {
            // Arrange
            var messageBody = new MessageBody
            {
                Counter = 0,
                Timestamp = DateTime.ParseExact("2024-01-01 00:00:01", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            // Act
            var result = Lib.Consumer.BusinessLogic.RuleApplication.Requirement2(messageBody);

            // Assert
            Assert.IsFalse(result);
        }

    }
}
