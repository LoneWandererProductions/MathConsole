using System.Collections.Generic;
using Interpreter;
using MatrixPlugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FunctionTest
{
    /// <summary>
    ///     Some basic function checks
    /// </summary>
    [TestClass]
    public class FunctionTests
    {
        /// <summary>
        ///     Sets the matrix valid input returns success message.
        /// </summary>
        [TestMethod]
        public void SetMatrixValidInputReturnsSuccessMessage()
        {
            // Arrange
            var parameter = new List<string> { "1", "2", "2", "1", "2", "3", "4" };

            var com = new OutCommand
            {
                Command = 0,
                Parameter = parameter
            };

            // Act
            var result = MatrixHandler.HandleCommands(com);

            // Assert
            Assert.AreEqual("Matrix added at Position: 1", result);
        }

        /// <summary>
        ///     Sets the matrix invalid parameter returns error message.
        /// </summary>
        [TestMethod]
        public void SetMatrixInvalidParameterReturnsErrorMessage()
        {
            // Arrange
            var parameter = new List<string> { "one", "2", "2", "1", "2", "3", "4" };

            var com = new OutCommand
            {
                Command = 0,
                Parameter = parameter
            };

            // Act
            var result = MatrixHandler.HandleCommands(com);

            // Assert
            Assert.AreEqual("Invalid parameter: id", result);
        }
    }
}