﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/InterpretInternal.cs
 * PURPOSE:     Tests for the internals of the Interpreter
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
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
        ///     The out command object for capturing command outputs.
        /// </summary>
        private static OutCommand _outCommand;

        /// <summary>
        ///     The log
        /// </summary>
        private static string _log;

        /// <summary>
        /// Gets the sample commands.
        /// </summary>
        /// <returns>Basic Command set</returns>
        private static IReadOnlyDictionary<int, InCommand> GetSampleCommands()
        {
            return new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "CMD1" } },
                { 2, new InCommand { Command = "CMD2" } }
            };
        }

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
            const string input = "(abc)";
            var result = Irt.RemoveParenthesis(input, '(', ')');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Tests if RemoveParenthesis returns the input string when no outer parentheses are present.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnInput()
        {
            const string input = "abc";
            var result = Irt.RemoveParenthesis(input, '(', ')');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Tests if RemoveParenthesis returns an error message when input has mismatched parentheses.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnErrorMessageWhenInputHasMismatchedParentheses()
        {
            const string input = "(abc";
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
        public void CheckForKeyWordWithNonExistingCommandReturnsError()
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
            const string input = "{abc}";
            var result = Irt.RemoveParenthesis(input, '{', '}');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Checks for key word with existing command returns correct key.
        /// </summary>
        [TestMethod]
        public void CheckForKeyWordWithExistingCommandReturnsCorrectKey()
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
        public void SplitParameterEmptyParametersShouldRemoveEmptyParameters()
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
            const string expected = "test().    Commmand()";
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
                { 0, new InCommand { Command = "com1", ParameterCount = 2, Description = "Help com1" } },
                {
                    1,
                    new InCommand { Command = "com2", ParameterCount = 0, Description = "com2 Command Namespace 1" }
                },
                {
                    2,
                    new InCommand { Command = "com3", ParameterCount = 0, Description = "Special case no Parameter" }
                }
            };

            var dctCommandTwo = new Dictionary<int, InCommand>
            {
                {
                    1,
                    new InCommand { Command = "com2", ParameterCount = 0, Description = "com2 Command Namespace 2" }
                },
                { 4, new InCommand { Command = "Test", ParameterCount = 0, Description = "Here we go" } }
            };

            var prompt = new Prompt();
            prompt.SendLogs += SendLogs;
            prompt.SendCommands += SendCommands;
            prompt.Initiate(dctCommandOne, "UserSpace 1");
            prompt.ConsoleInput("coM1(1,2)");

            Assert.AreEqual(0, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            prompt.AddCommands(dctCommandTwo, "UserSpace 2");

            Assert.AreEqual(2, prompt.CollectedSpaces.Count, "Wrong Number of Namespaces");

            prompt.ConsoleInput("use (UserSpace 2)");
            prompt.ConsoleInput("test");

            Assert.AreEqual(4, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            prompt.ConsoleInput("com2().use(UserSpace 1)");
            Assert.AreEqual(1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual("UserSpace 1", _outCommand.UsedNameSpace, "Wrong Userspace");

            var extension = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "Ext", ParameterCount = 0, Description = "Null" } },
                { 4, new InCommand { Command = "Ext", ParameterCount = 1, Description = "Overload" } }
            };

            // Reboot this time with user extension, check if we support overloads
            prompt.Initiate(dctCommandOne, "UserSpace 1", extension);
            prompt.ConsoleInput("com1(1,2).ext()");
            Assert.AreEqual(1, _outCommand.ExtensionCommand.ExtensionCommand, "Wrong Id: ");
            prompt.ConsoleInput("com1(1,2).ext(3)");
            Assert.AreEqual(4, _outCommand.ExtensionCommand.ExtensionCommand, "Wrong Id: ");
        }

        /// <summary>
        ///     Gets the parameters with advanced open returns batch command.
        /// </summary>
        [TestMethod]
        public void GetParametersWithAdvancedOpenReturnsBatchCommand()
        {
            // Arrange
            const string input = "CMD1{param}";
            const int key = 1;
            var commands = GetSampleCommands();

            // Act
            var (status, parameter) = Irt.GetParameters(input, key, commands);

            // Assert
            Assert.AreEqual(IrtConst.BatchCommand, status);
            Assert.AreEqual("{param}", parameter);
        }

        /// <summary>
        ///     Gets the parameters with normal parameters returns parameter command.
        /// </summary>
        [TestMethod]
        public void GetParametersWithNormalParametersReturnsParameterCommand()
        {
            // Arrange
            const string input = "CMD1(param)";
            const int key = 1;
            var commands = GetSampleCommands();

            // Act
            var (status, parameter) = Irt.GetParameters(input, key, commands);

            // Assert
            Assert.AreEqual(IrtConst.ParameterCommand, status);
            Assert.AreEqual("param", parameter);
        }

        /// <summary>
        ///     Gets the parameters with whitespace returns parameter command.
        /// </summary>
        [TestMethod]
        public void GetParametersWithWhitespaceReturnsParameterCommand()
        {
            // Arrange
            const string input = "   CMD1   (   param   )   ";
            const int key = 1;
            var commands = GetSampleCommands();

            // Act
            var (status, parameter) = Irt.GetParameters(input, key, commands);

            // Assert
            Assert.AreEqual(IrtConst.ParameterCommand, status);
            Assert.AreEqual("param", parameter);
        }

        /// <summary>
        ///     Checks the format valid format returns true.
        /// </summary>
        [TestMethod]
        public void CheckFormatValidFormatReturnsTrue()
        {
            // Arrange
            const string input = "command(label)";
            const string command = "command";
            const string label = "label";

            // Act
            var result = Irt.CheckFormat(input, command, label);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        ///     Checks the format invalid format returns false.
        /// </summary>
        [TestMethod]
        public void CheckFormatInvalidFormatReturnsFalse()
        {
            // Arrange
            const string input = "invalid(label)";
            const string command = "command";
            const string label = "label";

            // Act
            var result = Irt.CheckFormat(input, command, label);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Checks the format empty input returns false.
        /// </summary>
        [TestMethod]
        public void CheckFormatEmptyInputReturnsFalse()
        {
            // Arrange
            const string input = "";
            const string command = "command";
            const string label = "label";

            // Act
            var result = Irt.CheckFormat(input, command, label);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Checks the format null input returns false.
        /// </summary>
        [TestMethod]
        public void CheckFormatNullInputReturnsFalse()
        {
            // Arrange
            const string command = "command";
            const string label = "label";

            // Act
            var result = Irt.CheckFormat(null, command, label);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Checks the format different label returns false.
        /// </summary>
        [TestMethod]
        public void CheckFormatDifferentLabelReturnsFalse()
        {
            // Arrange
            const string input = "command(otherlabel)";
            const string command = "command";
            const string label = "label";

            // Act
            var result = Irt.CheckFormat(input, command, label);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests the first if without else.
        /// </summary>
        [TestMethod]
        public void TestFirstIfWithoutElse()
        {
            var input = "if(condition) {com1; com2;com3;} com1; com1; com1;";
            var expected = 31; // Position of the last '}'
            var actual = IfElseParser.FindLastClosingBracket(input);
            Assert.AreEqual(expected, actual);

            Trace.WriteLine(input[expected]);
        }

        /// <summary>
        /// Tests the first if with else.
        /// </summary>
        [TestMethod]
        public void TestFirstIfWithElse()
        {
            var input = "if(condition) {com1; com2;com3;} else {com1; com1; com1;} com1; com1; com1;";
            var expected = 56; // Position of the last '}'
            var actual = IfElseParser.FindLastClosingBracket(input);
            Assert.AreEqual(expected, actual);

            Trace.WriteLine(input[expected]);

            var result = IfElseParser.ExtractFirstIfElse(input);

            Trace.WriteLine(result);

            Assert.AreEqual(expected + 1, result.Length);

            Trace.WriteLine(input[expected]);
        }

        /// <summary>
        /// Tests the nested if else.
        /// </summary>
        [TestMethod]
        public void TestNestedIfElse()
        {
            var input = "if(condition) {com1; com2;com3;} else {if(condition) {com1; com2;com3;} else {com1; com1; com1;} com1; com1; } com1;";
            var expected = 109; // Position of the last '}'
            var actual = IfElseParser.FindLastClosingBracket(input);
            Assert.AreEqual(expected, actual);

            var result = IfElseParser.ExtractFirstIfElse(input);

            Trace.WriteLine(result);

            Assert.AreEqual(expected+1, result.Length);

            Trace.WriteLine(input[expected]);
        }

        [TestMethod]
        public void TestIfElseIfElse()
        {
            var input = "if(condition) {com1; com2;com3;} else {com1; com1; com1;} com1; com1; com1; if(condition) {com1; com2;com3;} else {com1; com1; com1;}";
            var expected = 57; // Position of the last '}'
            var actual = IfElseParser.FindLastClosingBracket(input);

            var result =IfElseParser.ExtractFirstIfElse(input);

            Trace.WriteLine(result);

            Assert.AreEqual(expected, result.Length);

            Trace.WriteLine(input[expected]);
        }

        /// <summary>
        /// Parses the most simple if else statement returns correct if else block.
        /// </summary>
        [TestMethod]
        public void ParseMostSimpleIfElseStatementReturnsCorrectIfElseBlock()
        {
            // Arrange
            var inputParts = new List<string> { "if(condition){com1", "}else {com2","}" };

            // Act
            var result = IfElseParser.Parse(inputParts);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("com1", result.IfClause);
            Assert.AreEqual("com2", result.ElseClause);
        }


        /// <summary>
        /// Parses the most simple if else statement returns correct if else block.
        /// </summary>
        [TestMethod]
        public void ParseMostIfElseCheckCornerCases()
        {
            // Arrange
            var inputParts = new List<string> { "if(condition){com1", "}else {com2", "} com3", "com4" };

            // Act
            var result = IfElseParser.Parse(inputParts);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("com1", result.IfClause);
            Assert.AreEqual("com2", result.ElseClause);
        }

        /// <summary>
        /// Parses the simple if else statement returns correct if else block.
        /// </summary>
        [TestMethod]
        public void ParseSimpleIfElseStatementReturnsCorrectIfElseBlock()
        {
            // Arrange
            var inputParts = new List<string> { "if(condition){com1", "com2", "com3}", "else {com4}" };

            // Act
            var result = IfElseParser.Parse(inputParts);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("com1 com2 com3", result.IfClause);
            Assert.AreEqual("com4", result.ElseClause);
        }

        /// <summary>
        /// Parses the nested if else statement returns correct if else block.
        /// </summary>
        [TestMethod]
        public void ParseNestedIfElseStatementReturnsCorrectIfElseBlock()
        {
            // Arrange
            var inputParts = new List<string> { "if(cond1){com1", "if(cond2){com2", "} else {com3}", "} else {com4}" };

            // Act
            var result = IfElseParser.Parse(inputParts);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("cond1", result.Condition);
            Assert.AreEqual("com1 if(cond2) { com2 } else { com3 }", result.IfClause);
            Assert.AreEqual("com4", result.ElseClause);
        }

        /// <summary>
        /// Parses the invalid input throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseInvalidInputThrowsException()
        {
            // Arrange
            var inputParts = new List<string> { "if(condition){com1", "com2", "com3", "else {com4" };

            // Act
            IfElseParser.Parse(inputParts);
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
            if (e.Command == -1) return;

            _outCommand = e;
        }
    }
}