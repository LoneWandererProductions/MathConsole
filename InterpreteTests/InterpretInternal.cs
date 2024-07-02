﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/InterpretInternal.cs
 * PURPOSE:     Tests for the internals of the Interpreter
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

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
        ///     The out command object for capturing command outputs.
        /// </summary>
        private static OutCommand _outCommand;

        /// <summary>
        /// The log
        /// </summary>
        private static string _log;

        /// <summary>
        ///     Tests if SingleCheck correctly identifies balanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnTrueForBalancedParentheses()
        {
            const string input = "(a + b) * c";
            var result = Irt.SingleCheck(input);
            Assert.IsTrue(result);
        }

        /// <summary>
        ///     Tests if SingleCheck correctly identifies unbalanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnFalseForUnbalancedParentheses()
        {
            const string input = "(a + b * c";
            var result = Irt.SingleCheck(input);
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Tests if RemoveParenthesis correctly removes outer parentheses when well-formed.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldRemoveOuterParenthesesWhenWellFormed()
        {
            var input = "(abc)";
            var result = Irt.RemoveParenthesis(input, '(', ')');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Tests if RemoveParenthesis returns the input string when no outer parentheses are present.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnInput()
        {
            var input = "abc";
            var result = Irt.RemoveParenthesis(input, '(', ')');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Tests if RemoveParenthesis returns an error message when input has mismatched parentheses.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnErrorMessageWhenInputHasMismatchedParentheses()
        {
            var input = "(abc";
            var result = Irt.RemoveParenthesis(input, '(', ')');
            Assert.AreEqual(IrtConst.ParenthesisError, result);
        }

        /// <summary>
        ///     Tests if CheckOverload returns the correct command identifier when overload matches.
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
        ///     Tests if CheckOverload returns null when overload does not match.
        /// </summary>
        [TestMethod]
        public void CheckOverloadShouldReturnErrorWhenOverloadDoesNotMatch()
        {
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "command", ParameterCount = 2 } }
            };

            var result = Irt.CheckOverload("command", 3, commands);
            Assert.IsNull(result);
        }

        /// <summary>
        ///     Tests if WellFormedParenthesis correctly removes well-formed parentheses.
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
        ///     Removes the parenthesis with malformed parentheses returns error.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisWithMalformedParenthesesReturnsError()
        {
            var result = Irt.RemoveParenthesis("(a + b", '(', ')');

            Assert.AreEqual(IrtConst.ParenthesisError, result);
        }


        /// <summary>
        ///     Checks for key word with non existing command returns error.
        /// </summary>
        [TestMethod]
        public void CheckForKeyWordWithNonExistingCommand_ReturnsError()
        {
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "TEST" } }
            };

            var result = Irt.CheckForKeyWord("INVALID", commands);

            Assert.AreEqual(IrtConst.Error, result);
        }

        /// <summary>
        ///     Singles the check should return true for nested balanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnTrueForNestedBalancedParentheses()
        {
            const string input = "((a + b) * (c - d))";
            var result = Irt.SingleCheck(input);
            Assert.IsTrue(result);
        }

        /// <summary>
        ///     Singles the check should return false for different count unbalanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnFalseForDifferentCountUnbalancedParentheses()
        {
            const string input = "(a + b) * (c - d";
            var result = Irt.SingleCheck(input);
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Checks the multiple should return true for balanced multiple parentheses.
        /// </summary>
        [TestMethod]
        public void CheckMultipleShouldReturnTrueForBalancedMultipleParentheses()
        {
            const string input = "{(a + b) * [c - d]}";
            var openParenthesis = new[] { '(', '{', '[' };
            var closeParenthesis = new[] { ')', '}', ']' };
            var result = Irt.CheckMultiple(input, openParenthesis, closeParenthesis);
            Assert.IsTrue(result);
        }

        /// <summary>
        ///     Checks the multiple should return false for unbalanced multiple parentheses.
        /// </summary>
        [TestMethod]
        public void CheckMultipleShouldReturnFalseForUnbalancedMultipleParentheses()
        {
            const string input = "{(a + b) * [c - d}";
            var openParenthesis = new[] { '(', '{', '[' };
            var closeParenthesis = new[] { ')', '}', ']' };
            var result = Irt.CheckMultiple(input, openParenthesis, closeParenthesis);
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Removes the parenthesis should remove outer curly braces when well formed.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldRemoveOuterCurlyBracesWhenWellFormed()
        {
            var input = "{abc}";
            var result = Irt.RemoveParenthesis(input, '{', '}');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Checks for key word with existing command returns correct key.
        /// </summary>
        [TestMethod]
        public void CheckForKeyWordWithExistingCommand_ReturnsCorrectKey()
        {
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "TEST" } },
                { 2, new InCommand { Command = "VALID" } }
            };

            var result = Irt.CheckForKeyWord("VALID", commands);
            Assert.AreEqual(2, result);
        }

        /// <summary>
        ///     Splits the parameter single parameter should return single parameter.
        /// </summary>
        [TestMethod]
        public void SplitParameterSingleParameterShouldReturnSingleParameter()
        {
            var result = Irt.SplitParameter("parameter1", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1" }, result);
        }

        /// <summary>
        ///     Splits the parameter multiple parameters should return all parameters.
        /// </summary>
        [TestMethod]
        public void SplitParameterMultipleParametersShouldReturnAllParameters()
        {
            var result = Irt.SplitParameter("parameter1,parameter2,parameter3", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter2", "parameter3" }, result);
        }

        /// <summary>
        ///     Splits the parameter with spaces should trim spaces.
        /// </summary>
        [TestMethod]
        public void SplitParameterWithSpacesShouldTrimSpaces()
        {
            var result = Irt.SplitParameter(" parameter1 , parameter2 , parameter3 ", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter2", "parameter3" }, result);
        }

        /// <summary>
        ///     Splits the parameter empty parameters should remove empty parameters.
        /// </summary>
        [TestMethod]
        public void SplitParameterEmptyParameters_ShouldRemoveEmptyParameters()
        {
            var result = Irt.SplitParameter("parameter1,,parameter3", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter3" }, result);
        }

        /// <summary>
        ///     Splits the parameter empty and whitespace parameters should remove empty and whitespace parameters.
        /// </summary>
        [TestMethod]
        public void SplitParameterEmptyAndWhitespaceParametersShouldRemoveEmptyAndWhitespaceParameters()
        {
            var result = Irt.SplitParameter("parameter1, ,parameter3", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter3" }, result);
        }

        /// <summary>
        ///     Splits the parameter custom splitter should split by custom splitter.
        /// </summary>
        [TestMethod]
        public void SplitParameterCustomSplitterShouldSplitByCustomSplitter()
        {
            var result = Irt.SplitParameter("parameter1|parameter2|parameter3", '|');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter2", "parameter3" }, result);
        }

        /// <summary>
        ///     Samples the splitter.
        /// </summary>
        [TestMethod]
        public void SampleSplitter()
        {
            var result = Irt.SplitParameter("help;;;", ';');
            CollectionAssert.AreEqual(new List<string> { "help" }, result);
        }


        /// <summary>
        ///     Tests internal extension functionality.
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

            // Assert
            Assert.AreEqual(IrtConst.ParameterMismatch, extensions.Status, "Not handled correctly");

            // Arrange
            input = "   test().use(test) ";

            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            // Assert
            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");

            // Test more exotic displays of extension

            // Arrange
            input = "   test(.).use(test) ";

            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            // Assert
            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");
            Assert.AreEqual("test(.)", extensions.Extension.BaseCommand, "Not handled correctly");

            // Arrange
            input = "   test( /...) . use(test)  ";

            // Act
            result = Irt.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            // Assert
            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");
            Assert.AreEqual("test( /...)", extensions.Extension.BaseCommand, "Not handled correctly");
        }

        /// <summary>
        ///     Tests the namespace switching functionality of the interpreter.
        /// </summary>
        [TestMethod]
        public void ConsoleNameSpaceSwitch()
        {
            var dctCommandOne = new Dictionary<int, InCommand>
            {
                { 0, new InCommand { Command = "First", ParameterCount = 2, Description = "Help First" } },
                {
                    1,
                    new InCommand { Command = "Second", ParameterCount = 0, Description = "Second Command Namespace 1" }
                },
                {
                    2,
                    new InCommand { Command = "Third", ParameterCount = 0, Description = "Special case no Parameter" }
                }
            };

            var dctCommandTwo = new Dictionary<int, InCommand>
            {
                {
                    1,
                    new InCommand { Command = "Second", ParameterCount = 0, Description = "Second Command Namespace 2" }
                },
                { 4, new InCommand { Command = "Test", ParameterCount = 0, Description = "Here we go" } }
            };

            var prompt = new Prompt();
            prompt.SendLogs += SendLogs;
            prompt.SendCommands += SendCommands;
            prompt.Initiate(dctCommandOne, "UserSpace 1");
            prompt.ConsoleInput("FirSt(1,2)");

            Assert.AreEqual(0, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            prompt.AddCommands(dctCommandTwo, "UserSpace 2");

            Assert.AreEqual(2, prompt.CollectedSpaces.Count, "Wrong Number of Namespaces");

            prompt.ConsoleInput("use (UserSpace 2)");
            prompt.ConsoleInput("test");

            Assert.AreEqual(4, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            prompt.ConsoleInput("Second().use(UserSpace 1)");
            Assert.AreEqual(1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual("UserSpace 1", _outCommand.UsedNameSpace, "Wrong Userspace");

            var extension = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "Ext", ParameterCount = 0, Description = "Null" } },
                { 4, new InCommand { Command = "Ext", ParameterCount = 1, Description = "Overload" } }
            };

            // Reboot this time with user extension, check if we support overloads
            prompt.Initiate(dctCommandOne, "UserSpace 1", extension);
            prompt.ConsoleInput("First(1,2).ext()");
            Assert.AreEqual(1, _outCommand.ExtensionCommand.ExtensionCommand, "Wrong Id: ");
            prompt.ConsoleInput("First(1,2).ext(3)");
            Assert.AreEqual(4, _outCommand.ExtensionCommand.ExtensionCommand, "Wrong Id: ");
        }

        /// <summary>
        ///     Logs the messages.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Message string</param>
        private static void SendLogs(object sender, string e)
        {
            Trace.WriteLine(e);
            _log = e;
        }

        /// <summary>
        ///     Captures the commands.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">OutCommand object</param>
        private static void SendCommands(object sender, OutCommand e)
        {
            if (e.Command == -1)
            {
                return;
            }

            _outCommand = e;
        }
    }
}