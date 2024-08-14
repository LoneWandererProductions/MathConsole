﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        InterpreteTests/InterpretInternal.cs
 * PURPOSE:     Tests for the internals of the Interpreter
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
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
        ///     Gets the sample commands.
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
            var result = IrtKernel.SingleCheck(input);
            Assert.IsTrue(result);
        }

        /// <summary>
        ///     Tests if SingleCheck correctly identifies unbalanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnFalseForUnbalancedParentheses()
        {
            const string input = "(a + b * c";
            var result = IrtKernel.SingleCheck(input);
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Tests if RemoveParenthesis correctly removes outer parentheses when well-formed.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldRemoveOuterParenthesesWhenWellFormed()
        {
            const string input = "(abc)";
            var result = IrtKernel.RemoveParenthesis(input, '(', ')');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Tests if RemoveParenthesis returns the input string when no outer parentheses are present.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnInput()
        {
            const string input = "abc";
            var result = IrtKernel.RemoveParenthesis(input, '(', ')');
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        ///     Tests if RemoveParenthesis returns an error message when input has mismatched parentheses.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldReturnErrorMessageWhenInputHasMismatchedParentheses()
        {
            const string input = "(abc";
            var result = IrtKernel.RemoveParenthesis(input, '(', ')');
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

            var result = IrtKernel.CheckOverload("command", 2, commands);
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

            var result = IrtKernel.CheckOverload("command", 3, commands);
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
            var result = IrtKernel.WellFormedParenthesis(input);

            // Assert
            Assert.AreEqual(expected, result);

            // Arrange
            input = " text {   internal(12 ,,,44   ) ,  internal4(      ,5)   }  ";
            expected = "text{   internal(12 ,,,44   ),  internal4(      ,5)}";

            // Act
            result = IrtKernel.WellFormedParenthesis(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        ///     Removes the parenthesis with malformed parentheses returns error.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisWithMalformedParenthesesReturnsError()
        {
            var result = IrtKernel.RemoveParenthesis("(a + b", '(', ')');

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

            var result = IrtKernel.CheckForKeyWord("INVALID", commands);

            Assert.AreEqual(IrtConst.Error, result);
        }

        /// <summary>
        ///     Singles the check should return true for nested balanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnTrueForNestedBalancedParentheses()
        {
            const string input = "((a + b) * (c - d))";
            var result = IrtKernel.SingleCheck(input);
            Assert.IsTrue(result);
        }

        /// <summary>
        ///     Singles the check should return false for different count unbalanced parentheses.
        /// </summary>
        [TestMethod]
        public void SingleCheckShouldReturnFalseForDifferentCountUnbalancedParentheses()
        {
            const string input = "(a + b) * (c - d";
            var result = IrtKernel.SingleCheck(input);
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
            var result = IrtKernel.CheckMultiple(input, openParenthesis, closeParenthesis);
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
            var result = IrtKernel.CheckMultiple(input, openParenthesis, closeParenthesis);
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Removes the parenthesis should remove outer curly braces when well formed.
        /// </summary>
        [TestMethod]
        public void RemoveParenthesisShouldRemoveOuterCurlyBracesWhenWellFormed()
        {
            const string input = "{abc}";
            var result = IrtKernel.RemoveParenthesis(input, '{', '}');
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

            var result = IrtKernel.CheckForKeyWord("VALID", commands);
            Assert.AreEqual(2, result);
        }

        /// <summary>
        ///     Splits the parameter single parameter should return single parameter.
        /// </summary>
        [TestMethod]
        public void SplitParameterSingleParameterShouldReturnSingleParameter()
        {
            var result = IrtKernel.SplitParameter("parameter1", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1" }, result);
        }

        /// <summary>
        ///     Splits the parameter multiple parameters should return all parameters.
        /// </summary>
        [TestMethod]
        public void SplitParameterMultipleParametersShouldReturnAllParameters()
        {
            var result = IrtKernel.SplitParameter("parameter1,parameter2,parameter3", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter2", "parameter3" }, result);
        }

        /// <summary>
        ///     Splits the parameter with spaces should trim spaces.
        /// </summary>
        [TestMethod]
        public void SplitParameterWithSpacesShouldTrimSpaces()
        {
            var result = IrtKernel.SplitParameter(" parameter1 , parameter2 , parameter3 ", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter2", "parameter3" }, result);
        }

        /// <summary>
        ///     Splits the parameter empty parameters should remove empty parameters.
        /// </summary>
        [TestMethod]
        public void SplitParameterEmptyParametersShouldRemoveEmptyParameters()
        {
            var result = IrtKernel.SplitParameter("parameter1,,parameter3", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter3" }, result);
        }

        /// <summary>
        ///     Splits the parameter empty and whitespace parameters should remove empty and whitespace parameters.
        /// </summary>
        [TestMethod]
        public void SplitParameterEmptyAndWhitespaceParametersShouldRemoveEmptyAndWhitespaceParameters()
        {
            var result = IrtKernel.SplitParameter("parameter1, ,parameter3", ',');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter3" }, result);
        }

        /// <summary>
        ///     Splits the parameter custom splitter should split by custom splitter.
        /// </summary>
        [TestMethod]
        public void SplitParameterCustomSplitterShouldSplitByCustomSplitter()
        {
            var result = IrtKernel.SplitParameter("parameter1|parameter2|parameter3", '|');
            CollectionAssert.AreEqual(new List<string> { "parameter1", "parameter2", "parameter3" }, result);
        }

        /// <summary>
        ///     Samples the splitter.
        /// </summary>
        [TestMethod]
        public void SampleSplitter()
        {
            var result = IrtKernel.SplitParameter("help;;;", ';');
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
            var result = IrtKernel.WellFormedParenthesis(input);

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
            result = IrtKernel.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            // Assert
            Assert.AreEqual(IrtConst.ParameterMismatch, extensions.Status, "Not handled correctly");

            // Arrange
            input = "   test().use(test) ";

            // Act
            result = IrtKernel.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            // Assert
            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");

            // Test more exotic displays of extension

            // Arrange
            input = "   test(.).use(test) ";

            // Act
            result = IrtKernel.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            // Assert
            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");
            Assert.AreEqual("test(.)", extensions.Extension.BaseCommand, "Not handled correctly");

            // Arrange
            input = "   test( /...) . use(test)  ";

            // Act
            result = IrtKernel.WellFormedParenthesis(input);
            extensions = ext.CheckForExtension(result, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            // Assert
            Assert.AreEqual(IrtConst.ExtensionFound, extensions.Status, "Not handled correctly");
            Assert.AreEqual("test( /...)", extensions.Extension.BaseCommand, "Not handled correctly");
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
            var (status, parameter) = IrtKernel.GetParameters(input, key, commands);

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
            var (status, parameter) = IrtKernel.GetParameters(input, key, commands);

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
            var (status, parameter) = IrtKernel.GetParameters(input, key, commands);

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
            var result = IrtKernel.CheckFormat(input, command, label);

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
            var result = IrtKernel.CheckFormat(input, command, label);

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
            var result = IrtKernel.CheckFormat(input, command, label);

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
            var result = IrtKernel.CheckFormat(null, command, label);

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
            var result = IrtKernel.CheckFormat(input, command, label);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        ///     Tests the first if with else.
        /// </summary>
        [TestMethod]
        public void TestFirstIfWithElse()
        {
            var input = "if(condition) {com1; com2;com3;} else {com1; com1; com1;} com1; com1; com1;";
            var expected = 56; // Position of the last '}'
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected + 1, block.Length);
            Assert.AreEqual(33, elsePosition);
        }

        /// <summary>
        ///     Tests the nested if else.
        /// </summary>
        [TestMethod]
        public void TestNestedIfElse()
        {
            var input =
                "if(condition) {com1; com2;com3;} else {if(condition) {com1; com2;com3;} else {com1; com1; com1;} com1; com1; } com1;";
            var expected = 109; // Position of the last '}'
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected + 1, block.Length);
            Assert.AreEqual(33, elsePosition);
        }

        /// <summary>
        ///     Tests if else if else.
        /// </summary>
        [TestMethod]
        public void TestIfElseIfElse()
        {
            var input =
                "if(condition) {com1; com2;com3;} else {com1; com1; com1;} com1; com1; com1; if(condition) {com1; com2;com3;} else {com1; com1; com1;}";
            var expected = 57; // Position of the last '}'
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block.Length);
            Assert.AreEqual(33, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else simple nested if else returns correct block.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseSimpleNestedIfElseReturnsCorrectBlock()
        {
            var input = "if(condition) { if(innerCondition) { com1; } else { com2; } } else { com3; }";
            var expected = "if(condition) { if(innerCondition) { com1; } else { com2; } } else { com3; }";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(62, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else if without else returns if block.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseIfWithoutElseReturnsIfBlock()
        {
            var input = "if(condition) { com1; } com2; com3;";
            var expected = "if(condition) { com1; }";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(-1, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else multiple if else blocks returns first block.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseMultipleIfElseBlocksReturnsFirstBlock()
        {
            var input = "if(condition1) { com1; } else { com2; } if(condition2) { com3; } else { com4; }";
            var expected = "if(condition1) { com1; } else { com2; }";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(25, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else no if returns null.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseNoIfReturnsNull()
        {
            var input = "com1; com2; com3;";
            var result = IrtKernel.ExtractFirstIfElse(input);
            Assert.IsNull(result.block);
        }

        /// <summary>
        ///     Extracts the first if else multiple levels of nesting returns first block.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseMultipleLevelsOfNestingReturnsFirstBlock()
        {
            var input =
                "if(condition) { if(innerCondition1) { com1; } else { if(innerCondition2) { com2; } else { com3; } } } else { com4; }";
            var expected =
                "if(condition) { if(innerCondition1) { com1; } else { if(innerCondition2) { com2; } else { com3; } } } else { com4; }";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(102, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else if else with comments returns first block.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseIfElseWithCommentsReturnsFirstBlock()
        {
            var input = "if(condition) { /* comment */ com1; } else { // comment \n com2; } com3;";
            var expected = "if(condition) { /* comment */ com1; } else { // comment \n com2; }";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(38, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else else at end of string handles correctly.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseElseAtEndOfStringHandlesCorrectly()
        {
            var input = "if(condition) { com1; } else";
            var expected = "if(condition) { com1; } else";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(24, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else else without braces handles correctly.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseElseWithoutBracesHandlesCorrectly()
        {
            var input = "if(condition) { com1; } else com2;";
            var expected = "if(condition) { com1; } else com2;";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(24, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else mixed case returns correct block.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseMixedCaseReturnsCorrectBlock()
        {
            var input = "If(condition) { com1; } eLsE { com2; }";
            var expected = "If(condition) { com1; } eLsE { com2; }";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(24, elsePosition);
        }

        /// <summary>
        ///     Extracts the first if else upper case returns correct block.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseUpperCaseReturnsCorrectBlock()
        {
            var input = "IF(condition) { com1; } ELSE { com2; }";
            var expected = "IF(condition) { com1; } ELSE { com2; }";
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(24, elsePosition);
        }

        /// <summary>
        /// Extracts the first if else no else.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseNoElse()
        {
            var input = "if(condition) { com1; } com4;"; // Adjusted input string
            var expected = "if(condition) { com1; }"; // Expected output with no `else`
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(-1, elsePosition); // Since there's no `else`, the position should be -1
        }

        /// <summary>
        /// Extracts the nested if with no else.
        /// </summary>
        [TestMethod]
        public void ExtractNestedIfNoElse()
        {
            var input = "if (condition1){command1; if (condition2){command2;}}";
            var expected = "if (condition1){command1; if (condition2){command2;}}"; // Expected output with no `else`
            var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(input);
            Assert.AreEqual(expected, block);
            Assert.AreEqual(-1, elsePosition); // Since there's no `else`, the position should be -1
        }

        /// <summary>
        /// Extracts the first if else empty input returns null.
        /// </summary>
        [TestMethod]
        public void ExtractFirstIfElseEmptyInputReturnsNull()
        {
            var input = "";
            var result = IrtKernel.ExtractFirstIfElse(input);
            Assert.IsNull(result.block);
        }

        /// <summary>
        /// Validates the parameters with correct parameters returns true.
        /// </summary>
        [TestMethod]
        public void ValidateParametersWithCorrectParametersReturnsTrue()
        {
            // Arrange
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { ParameterCount = 2 } }
            };

            // Act
            var result = IrtKernel.ValidateParameters(1, 2, commands);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Validates the parameters with incorrect parameters returns false.
        /// </summary>
        [TestMethod]
        public void ValidateParametersWithIncorrectParametersReturnsFalse()
        {
            // Arrange
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { ParameterCount = 2 } }
            };

            // Act
            var result = IrtKernel.ValidateParameters(1, 3, commands);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Removes the first occurrence removes symbol.
        /// </summary>
        [TestMethod]
        public void RemoveFirstOccurrence_RemovesSymbol()
        {
            // Act
            var result = IrtKernel.RemoveFirstOccurrence("hello world", 'o');

            // Assert
            Assert.AreEqual("hell world", result);
        }

        /// <summary>
        /// Cuts the last occurrence removes last symbols.
        /// </summary>
        [TestMethod]
        public void CutLastOccurrenceRemovesLastSymbol()
        {
            // Act
            var result = IrtKernel.CutLastOccurrence("hello world", 'o');

            // Assert
            Assert.AreEqual("hello w", result);
        }

        /// <summary>
        /// Removes the word removes keyword.
        /// </summary>
        [TestMethod]
        public void RemoveWordRemovesKeyword()
        {
            // Act
            var result = IrtKernel.RemoveWord("keyword", "This is a keyword example.");

            // Assert
            Assert.AreEqual("This is a  example.", result);
        }

        /// <summary>
        /// Extracts the condition removes keyword and parentheses.
        /// </summary>
        [TestMethod]
        public void ExtractConditionRemovesKeywordAndParentheses()
        {
            // Act
            var result = IrtKernel.ExtractCondition("if (condition) something", "if");

            // Assert
            Assert.AreEqual("condition", result);
        }

        /// <summary>
        /// Finds the first if index finds keyword.
        /// </summary>
        [TestMethod]
        public void FindFirstIfIndexFindsKeyword()
        {
            // Act
            var result = IrtKernel.FindFirstKeywordIndex("some text if (condition)", "if");

            // Assert
            Assert.AreEqual(10, result);

            // Act
            result = IrtKernel.FindFirstKeywordIndex("some text if (condition)", "else");

            // Assert
            Assert.AreEqual(-1, result);
        }

        /// <summary>
        /// Determines whether [contains keyword with open paren finds keyword with paren].
        /// </summary>
        [TestMethod]
        public void ContainsKeywordWithOpenParenFindsKeywordWithParen()
        {
            // Act
            var result = IrtKernel.ContainsKeywordWithOpenParen("some text if (condition)", "if");

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Gets the command index finds matching command.
        /// </summary>
        [TestMethod]
        public void GetCommandIndexFindsMatchingCommand()
        {
            // Arrange
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "COMMAND" } }
            };

            // Act
            var result = IrtKernel.GetCommandIndex("COMMAND", commands);

            // Assert
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Gets the command index does not find matching command.
        /// </summary>
        [TestMethod]
        public void GetCommandIndexDoesNotFindMatchingCommand()
        {
            // Arrange
            var commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "COMMAND" } }
            };

            // Act
            var result = IrtKernel.GetCommandIndex("UNKNOWN", commands);

            // Assert
            Assert.AreEqual(IrtConst.Error, result);
        }
    }
}