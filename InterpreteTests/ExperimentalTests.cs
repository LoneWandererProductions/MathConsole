/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/ExperimentalTests.cs
 * PURPOSE:     Tests for experimental stuff unused for now
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterpreteTests
{
    /// <summary>
    ///     Interpreter unit test class. Internal functions
    /// </summary>
    [TestClass]
    public sealed class ExperimentalTests
    {
        /// <summary>
        ///     Parses the most simple if else statement returns correct if else block.
        /// </summary>
        [TestMethod]
        public void ParseMostSimpleIfElseStatementReturnsCorrectIfElseBlock()
        {
            var inputParts = new List<string> {"if(condition){com1", "}else {com2", "}"};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("com1", result.IfClause);
            Assert.AreEqual("com2", result.ElseClause);
        }

        /// <summary>
        ///     Parses the most if else check corner cases.
        /// </summary>
        [TestMethod]
        public void ParseMostIfElseCheckCornerCases()
        {
            var inputParts = new List<string> {"if(condition){com1", "}else {com2", "} com3", "com4"};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("com1", result.IfClause);
            Assert.AreEqual("com2", result.ElseClause);
        }

        /// <summary>
        ///     Parses the simple if else statement returns correct if else block.
        /// </summary>
        [TestMethod]
        public void ParseSimpleIfElseStatementReturnsCorrectIfElseBlock()
        {
            var inputParts = new List<string> {"if(condition){com1", "com2", "com3}", "else {com4}"};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("com1 com2 com3", result.IfClause);
            Assert.AreEqual("com4", result.ElseClause);
        }

        /// <summary>
        ///     Parses the nested if else statement returns correct if else block.
        /// </summary>
        [TestMethod]
        public void ParseNestedIfElseStatementReturnsCorrectIfElseBlock()
        {
            var inputParts = new List<string> {"if(cond1){com1", "if(cond2){com2", "} else {com3}", "} else {com4}"};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("cond1", result.Condition);
            Assert.AreEqual("com1 if(cond2) { com2 } else { com3 }", result.IfClause);
            Assert.AreEqual("com4", result.ElseClause);
        }

        /// <summary>
        ///     Parses the invalid input throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseInvalidInputThrowsException()
        {
            var inputParts = new List<string> {"if(condition){com1", "com2", "com3", "else {com4"};
            Experimental.Parse(inputParts);
        }

        /// <summary>
        ///     Parses the single closing brace throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseSingleClosingBraceThrowsException()
        {
            var inputParts = new List<string> {"if(condition){com1", "com2}", "else"};
            Experimental.Parse(inputParts);
        }

        /// <summary>
        ///     Parses the multiple else keywords returns correct block.
        /// </summary>
        [TestMethod]
        public void ParseMultipleElseKeywordsReturnsCorrectBlock()
        {
            var inputParts = new List<string> {"if(condition1){com1", "}else{if(condition2){com2", "}else{com3", "}}}"};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("condition1", result.Condition);
            Assert.AreEqual("com1", result.IfClause);
            Assert.AreEqual("if(condition2) { com2 } else { com3 }", result.ElseClause);
        }

        /// <summary>
        ///     Parses the adjacent if else blocks returns correct block.
        /// </summary>
        [TestMethod]
        public void ParseAdjacentIfElseBlocksReturnsCorrectBlock()
        {
            var inputParts = new List<string> {"if(cond1){com1", "}else{if(cond2){com2", "}else{com3", "}}}"};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("cond1", result.Condition);
            Assert.AreEqual("com1", result.IfClause);
            Assert.AreEqual("if(cond2) { com2 } else { com3 }", result.ElseClause);
        }

        /// <summary>
        ///     Parses if without else returns correct block.
        /// </summary>
        [TestMethod]
        public void ParseIfWithoutElseReturnsCorrectBlock()
        {
            var inputParts = new List<string> {"if(condition){com1", "com2", "com3}"};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("com1 com2 com3", result.IfClause);
            Assert.AreEqual(null, result.ElseClause);
        }


        /// <summary>
        ///     Parses the extra whitespaces returns correct block.
        /// </summary>
        [TestMethod]
        public void ParseExtraWhitespacesReturnsCorrectBlock()
        {
            var inputParts = new List<string> {"  if ( condition ) {  com1;  ", "com2; } else {  com3; }  "};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("com1; com2;", result.IfClause);
            Assert.AreEqual("com3;", result.ElseClause);
        }


        /// <summary>
        ///     Parses if else with comments returns correct block.
        /// </summary>
        [TestMethod]
        public void ParseIfElseWithCommentsReturnsCorrectBlock()
        {
            var inputParts = new List<string>
                {"if(condition) { /* comment */ com1", "com2", "} else { // comment", "com3 }"};
            var result = Experimental.Parse(inputParts);
            Assert.IsNotNull(result);
            Assert.AreEqual("condition", result.Condition);
            Assert.AreEqual("/* comment */ com1 com2", result.IfClause);
            Assert.AreEqual("// comment com3", result.ElseClause);
        }
    }
}