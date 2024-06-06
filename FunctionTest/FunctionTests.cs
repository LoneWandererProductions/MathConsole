using Interpreter;
using MatrixPlugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FunctionTest
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void SetMatrix_ValidInput_ReturnsSuccessMessage()
        {
            // Arrange
            var parameter = new List<string> { "1", "2", "2", "1", "2", "3", "4" };

            var com = new OutCommand
            {
                Command = 0,
                Parameter = parameter
            };

            // Act
            string result = MatrixHandler.HandleCommands(com);

            // Assert
            Assert.AreEqual("Matrix added at Position: 1", result);
        }

        [TestMethod]
        public void SetMatrix_InvalidParameter_ReturnsErrorMessage()
        {
            // Arrange
            var parameter = new List<string> { "one", "2", "2", "1", "2", "3", "4" };

            var com = new OutCommand
            {
                Command = 0,
                Parameter = parameter
            };

            // Act
            string result = MatrixHandler.HandleCommands(com);

            // Assert
            Assert.AreEqual("Invalid parameter: id", result);
        }
    }
}
