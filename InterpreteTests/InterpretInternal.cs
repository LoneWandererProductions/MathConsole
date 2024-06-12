/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/InterpretInternal.cs
 * PURPOSE:     Tests for the internals of theInterpreter
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// The log
        /// </summary>
        private static string _log;

        /// <summary>
        /// The out command
        /// </summary>
        private static OutCommand _outCommand;

        /// <summary>
        ///     Singles the check should return true for balanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnTrueForBalancedParentheses()
        {
            const string input = "(a + b) * c";
            var result = Irt.SingleCheck(input);
            Assert.IsTrue(result);
        }

        /// <summary>
        ///     Singles the check should return false for unbalanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnFalseForUnbalancedParentheses()
        {
            const string input = "(a + b * c";
            var result = Irt.SingleCheck(input);
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Removes the parenthesis should remove outer parentheses when well formed.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldRemoveOuterParenthesesWhenWellFormed()
        {
            var input = "(abc)";
            var result = Irt.RemoveParenthesis(input, '(', ')');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Removes the parenthesis should return the input string, this is just an edge Case
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnInput()
        {
            var input = "abc";
            var result = Irt.RemoveParenthesis(input, ')', '(');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Removes the parenthesis should return error message when input has mismatched parentheses.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnErrorMessageWhenInputHasMismatchedParentheses()
        {
            var input = "(abc";
            var result = Irt.RemoveParenthesis(input, ')', '(');
            Assert.AreEqual(IrtConst.ParenthesisError, result);
        }

        /// <summary>
        ///     Checks the overload should return command identifier when overload matches.
        /// </summary>
        [TestMethod]
        public void CheckOverloadShouldReturnCommandIdWhenOverloadMatches()
        {
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "command", ParameterCount = 2 } }
            };

            var result = Irt.CheckOverload("command", 2, commands);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        ///     Checks the overload should return error when overload does not match.
        /// </summary>
        [TestMethod]
        public void CheckOverloadShouldReturnErrorWhenOverloadDoesNotMatch()
        {
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "command", ParameterCount = 2 } }
            };

            var result = Irt.CheckOverload("command", 3, commands);
            Assert.AreEqual(null, result);
        }

        /// <summary>
        ///     Wells the formed parenthesis removes well formed parenthesis.
        /// </summary>
        [TestMethod]
        public void WellFormedParenthesisRemovesWellFormedParenthesis()
        {
            // Arrange
            var input = "text ) ( , ) txt ) ( , ( more text";
            var expected = "text )( , )txt )( ,( more text";

            // Act
            var result = Irt.WellFormedParenthesis(input);

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
            var input = "   test().    Commmand() ";
            var expected = "test().    Commmand()";
            var ext = new IrtExtension();
            // Act
            var result = Irt.WellFormedParenthesis(input);

            // Assert
            Assert.AreEqual(expected, result);

            // Act
            var extensions =
                ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);
            // Assert
            Assert.AreEqual(IrtConst.Error, extensions.Status, "Not handled correctly");

            // Arrange
            input = "   test().    use(     ) ";
            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            Assert.AreEqual(IrtConst.ParameterMismatch, extensions.Status, "Not handled correctly");

            // Arrange
            input = "   test().use(test) ";
            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");

            //test more exotic displays of extension

            // Arrange
            input = "   test(.).use(test) ";
            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");

            Assert.AreEqual("test(.)", extensions.Extension.BaseCommand, "Not handled correctly");

            // Arrange
            input = "   test( /...) . use(test)  ";
            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");

            Assert.AreEqual("test( /...)", extensions.Extension.BaseCommand, "Not handled correctly");
        }

        /// <summary>
        ///     Check our Interpreter
        /// </summary>
        [TestMethod]
        public void ConsoleNameSpaceSwitch()
        {
            var dctCommandOne = new Dictionary<int, InCommand> ()
            {
                { 0, new InCommand { Command = "First", ParameterCount = 2, Description = "Help First" } },
                { 1, new InCommand { Command = "Second", ParameterCount = 0, Description = "Second Command Namespace 1"  } },
                {
                    2,
                    new InCommand { Command = "Third", ParameterCount = 0, Description = "Special case no Parameter" }
                }
            };

            var  dctCommandTwo = new Dictionary<int, InCommand>()
            {
                { 1, new InCommand { Command = "Second", ParameterCount = 0, Description = "Second Command Namespace 2" } },
                { 4, new InCommand { Command = "Test", ParameterCount = 0, Description = "Here we go" } }
            };

            var prompt = new Prompt();
            prompt.SendLogs += SendLogs;
            prompt.SendCommands += SendCommands;
            prompt.Initiate(dctCommandOne, "UserSpace 1");
            prompt.StartConsole("FirSt(1,2)");

            Assert.AreEqual(0, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            prompt.AddCommands(dctCommandTwo, "UserSpace 2");

            Assert.AreEqual(2, prompt.CollectedSpaces.Count, "Wrong Number of Namespaces");

            prompt.StartConsole("use (UserSpace 2)");
            prompt.StartConsole("test");

            Assert.AreEqual(4, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            prompt.StartConsole("Second().use(UserSpace 1)");
            Assert.AreEqual(1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual("UserSpace 1", _outCommand.UsedNameSpace, "Wrong Userspace");
        }

        /// <summary>
        ///     Listen to Messages
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Type</param>
        private static void SendLogs(object sender, string e)
        {
            Trace.WriteLine(e);
            _log = e;
        }

        /// <summary>
        ///     Listen to Commands
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Type</param>
        private static void SendCommands(object sender, OutCommand e)
        {
            if (e.Command == -1) _log = e.ErrorMessage;
            _outCommand = e;
        }
    }
}