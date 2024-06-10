/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/InterpretInternal.cs
 * PURPOSE:     Tests for the internals of theInterpreter
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterpreteTests
{
    /// <summary>
    ///     Interpreter unit test class. Internal functions
    /// </summary>
    [TestClass]
    public sealed class InterpretInternal
    {
        /// <summary>
        ///     Singles the check should return true for balanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnTrueForBalancedParentheses()
        {
            const string input = "(a + b) * c";
            bool result = Irt.SingleCheck(input);
            Assert.IsTrue(result);
        }

        /// <summary>
        ///     Singles the check should return false for unbalanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnFalseForUnbalancedParentheses()
        {
            const string input = "(a + b * c";
            bool result = Irt.SingleCheck(input);
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Removes the parenthesis should remove outer parentheses when well formed.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldRemoveOuterParenthesesWhenWellFormed()
        {
            string input = "(abc)";
            string result = Irt.RemoveParenthesis(input, ')', '(');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Removes the parenthesis should return the input string, this is just an edge Case
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnInput()
        {
            string input = "abc";
            string result = Irt.RemoveParenthesis(input, ')', '(');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Removes the parenthesis should return error message when input has mismatched parentheses.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnErrorMessageWhenInputHasMismatchedParentheses()
        {
            string input = "(abc";
            string result = Irt.RemoveParenthesis(input, ')', '(');
            Assert.AreEqual(IrtConst.ParenthesisError, result);
        }

        /// <summary>
        ///     Checks the overload should return command identifier when overload matches.
        /// </summary>
        [TestMethod]
        public void CheckOverloadShouldReturnCommandIdWhenOverloadMatches()
        {
            Dictionary<int, InCommand> commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "command", ParameterCount = 2 } }
            };

            int? result = Irt.CheckOverload("command", 2, commands);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        ///     Checks the overload should return error when overload does not match.
        /// </summary>
        [TestMethod]
        public void CheckOverloadShouldReturnErrorWhenOverloadDoesNotMatch()
        {
            Dictionary<int, InCommand> commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "command", ParameterCount = 2 } }
            };

            int? result = Irt.CheckOverload("command", 3, commands);
            Assert.AreEqual(null, result);
        }

        /// <summary>
        ///     Wells the formed parenthesis removes well formed parenthesis.
        /// </summary>
        [TestMethod]
        public void WellFormedParenthesisRemovesWellFormedParenthesis()
        {
            // Arrange
            string input = "text ) ( , ) txt ) ( , ( more text";
            string expected = "text )( , )txt )( ,( more text";

            // Act
            string result = Irt.WellFormedParenthesis(input);

            // Assert
            Assert.AreEqual(expected, result);

            // Arrange
            input = " text {   internal(12 ,,,44   ) ,  internal4(      ,5)   }  ";
            expected = "text{   internal(12 ,,,44   ),  internal4(      ,5)}";

            // Act
            result = Irt.WellFormedParenthesis(input);

            // Assert
            Assert.AreEqual(expected, result);
        }


        /// <summary>
        ///     Wells the formed parenthesis removes well formed parenthesis.
        /// </summary>
        [TestMethod]
        public void InternalExtensionTest()
        {
            // Arrange
            string input = "   test().    Commmand() ";
            string expected = "test().    Commmand()";
            IrtExtension ext = new IrtExtension();
            // Act
            string result = Irt.WellFormedParenthesis(input);

            // Assert
            Assert.AreEqual(expected, result);

            // Act
            (ExtensionCommands Extension, int Status) extensions =
                ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);
            // Assert
            Assert.AreEqual(-1, extensions.Status, "Not handled correctly");

            // Arrange
            input = "   test().    use(     ) ";
            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            Assert.AreEqual(2, extensions.Status, "Not handled correctly");

            // Arrange
            input = "   test().use(test) ";
            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            Assert.AreEqual(3, extensions.Status, "Not handled correctly");
        }
    }
}