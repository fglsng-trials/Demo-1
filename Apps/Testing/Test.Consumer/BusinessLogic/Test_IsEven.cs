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
    public class Test_IsEven
    {
        /// <summary>
        /// Will test the IsEven method with an even number number.
        /// </summary>
        [TestMethod]
        public void Test_IsEven_True()
        {
            // Arrange
            int number = 2;

            // Act
            var result = Lib.Consumer.BusinessLogic.RuleApplication.IsEven(number);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Will test the IsEven method with an odd number number.
        /// </summary>
        [TestMethod]
        public void Test_IsEven_False()
        {
            // Arrange
            int number = 1;

            // Act
            var result = Lib.Consumer.BusinessLogic.RuleApplication.IsEven(number);

            // Assert
            Assert.IsFalse(result);
        }

    }
}
