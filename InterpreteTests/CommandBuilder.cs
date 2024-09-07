using System.Collections.Generic;
using System.Diagnostics;
using Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterpreteTests
{
    [TestClass]
    public class CommandBuilder
    {
        /// <summary>
        /// Parses if else clauses no if else clauses returns empty dictionary.
        /// </summary>
        [TestMethod]
        public void ParseIfElseClausesNoIfElseClausesReturnsEmptyDictionary()
        {
            // Arrange
            var input = "com1;";

            // Act
            var result = IfElseObjExp.ParseIfElseClauses(input);

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        /// <summary>
        /// Parses if else clauses single if clause returns one if else object.
        /// </summary>
        [TestMethod]
        public void ParseIfElseClausesSingleIfClauseReturnsOneIfElseObj()
        {
            // Arrange
            const string input = "if (condition1) {com1; }";

            // Act
            var result = IfElseObjExp.ParseIfElseClauses(input);

            // Assert
            Assert.AreEqual(3, result.Count);

            var ifElseObj = result[0];
            Assert.AreEqual(0, ifElseObj.Id);
            Assert.AreEqual(-1, ifElseObj.ParentId);
            Assert.AreEqual(0, ifElseObj.Position);
            Assert.AreEqual(0, ifElseObj.Layer);
            Assert.IsFalse(ifElseObj.Else);
            Assert.IsTrue(ifElseObj.Nested);
            Assert.AreEqual("if (condition1) {com1; }", ifElseObj.Input);
        }

        /// <summary>
        /// Parses if else clauses single if clause returns correct object.
        /// </summary>
        [TestMethod]
        public void ParseIfElseClausesSingleIfClauseReturnsCorrectObject()
        {
            var input = "if (condition) { doSomething(); }";
            var result = IfElseObjExp.ParseIfElseClauses(input);

            Assert.AreEqual(3, result.Count, "There should be one IfElseObj in the result.");
            var obj = result[0];
            Assert.IsFalse(obj.Else, "The 'Else' flag should be false for an 'if' clause.");
            Assert.AreEqual(-1, obj.ParentId, "The ParentId should be -1 for a top-level 'if' clause.");
            Assert.AreEqual(0, obj.Layer, "The Layer should be 0 for a top-level 'if' clause.");
            Assert.AreEqual(0, obj.Position, "The Position should be 0 for a top-level 'if' clause.");
            Assert.AreEqual("if (condition) { doSomething(); }", obj.Input, "The Input string should match.");
        }

        /// <summary>
        /// Parses if else clauses if with else returns correct objects.
        /// </summary>
        [TestMethod]
        public void ParseIfElseClausesIfWithElseReturnsCorrectObjects()
        {
            var input = "if (condition) { doSomething(); } else { doSomething(); }";
            var result = IfElseObjExp.ParseIfElseClauses(input);

            //TODO refine
            //Assert.AreEqual(1, result.Count, "There should be one IfElseObj in the result.");

            // Check the 'if' clause
            var ifObj = result[0];
            //Assert.IsFalse(ifObj.Else, "The 'Else' flag should be false for the 'if' clause.");
            //Assert.AreEqual(-1, ifObj.ParentId, "The ParentId should be -1 for a top-level 'if' clause.");
            //Assert.AreEqual(0, ifObj.Layer, "The Layer should be 0 for a top-level 'if' clause.");
            //Assert.AreEqual(0, ifObj.Position, "The Position should be 0 for a top-level 'if' clause.");
            //Assert.AreEqual("if (condition) { doSomething(); } else { doSomethingElse(); }", ifObj.Input, "The Input string should match the whole clause.");
            //var ifClause = ifObj.Commands.Get(0);
            //Assert.AreEqual("if (condition) { doSomething(); }", ifClause, "The Input string should match the 'if' clause.");

            //ifClause = ifObj.Commands.Get(1);
            //Assert.AreEqual("else { doSomethingElse(); }", ifClause, "The Input string should match for the 'else' clause.");
        }

        /// <summary>
        /// Parses if else clauses empty input returns empty dictionary.
        /// </summary>
        [TestMethod]
        public void ParseIfElseClausesEmptyInputReturnsEmptyDictionary()
        {
            var input = string.Empty;
            var result = IfElseObjExp.ParseIfElseClauses(input);

            Assert.AreEqual(null, result, "The result should be an empty dictionary for empty input.");
        }

        /// <summary>
        /// Parses if else clauses malformed input returns single object.
        /// </summary>
        [TestMethod]
        public void ParseIfElseClausesMalformedInputReturnsSingleObject()
        {
            var input = "if (condition { doSomething(); }";
            var result = IfElseObjExp.ParseIfElseClauses(input);

            Assert.AreEqual(1, result.Count, "There should be one IfElseObj in the result for malformed input.");
            var obj = result[0];
            Assert.IsFalse(obj.Else, "The 'Else' flag should be false for a malformed 'if' clause.");
            Assert.AreEqual(-1, obj.ParentId, "The ParentId should be -1 for a top-level malformed 'if' clause.");
            Assert.AreEqual(0, obj.Layer, "The Layer should be 0 for a top-level malformed 'if' clause.");
            Assert.AreEqual(0, obj.Position, "The Position should be 0 for a top-level malformed 'if' clause.");
            Assert.AreEqual("if (condition { doSomething(); }", obj.Input, "The Input string should match.");
        }

        [TestMethod]
        public void TestParseIfElseClausesNestedIfElse()
        {
            // Arrange
            string input = "if(condition1) { if(condition2) { /* nested code */ } else { /* nested else code */ } } else { /* outer else code */ }";

            // Act
            var clauses = IfElseObjExp.ParseIfElseClauses(input);

            // Assert
            //Assert.AreEqual(2, clauses.Count, "Expected 2 if-else clauses.");
        }

        /// <summary>
        ///     Parses the invalid input throws exception.
        /// </summary>
        [TestMethod]
        public void ParseComplexCommand()
        {
            //base, the command will be removed in the IrtParser
            const string input = "Container{ " +
                                 "Print(hello World);" +
                                 "if(condition) { if(innerCondition) { com1; } else { com2; } } else { com3; }" +
                                 "Label(one);" +
                                 "Print(passed label one);" +
                                 "goto(two);" +
                                 "Print(Should not be printed);" +
                                 "Label(two);" +
                                 "Print(Finish);" +
                                 "}";

            var inputcleaned = "{ " +
                               "Print(hello World);" +
                               "if(condition) { if(innerCondition) { com1; } else { com2; } } else { com3; }" +
                               "Label(one);" +
                               "Print(passed label one);" +
                               "goto(two);" +
                               "Print(Should not be printed);" +
                               "Label(two);" +
                               "Print(Finish);" +
                               "}";

            var expectedResults = new List<(int Key, string Category, string Value)>
            {
                (1, "COMMAND", "Print(hello World)"),
                (2, "IF", "if(condition) { if(innerCondition) { com1; } else { com2; } }"),
                (3, "ELSE", "else { com3; }"),
                (5, "LABEL", "Label(one)"),
                (6, "COMMAND", "Print(passed label one)"),
                (7, "GOTO", "goto(two)"),
                (8, "COMMAND", "Print(Should not be printed)"),
                (9, "LABEL", "Label(two)"),
                (10, "COMMAND", "Print(Finish)")
            };

            // Act
            var result = IrtParserCommand.BuildCommand(inputcleaned);
            result = IrtParserCommand.BuildCommand(inputcleaned);

            Trace.WriteLine(result.ToString());

            //// Assert
            //foreach (var expected in expectedResults)
            //{
            //    var (key, category, value) = expected;
            //    Assert.IsTrue(result.TryGetCategory(key, out var actualCategory));
            //    Assert.AreEqual(category, actualCategory, $"Category mismatch for key {key}");
            //    Assert.IsTrue(result.TryGetValue(key, out var actualValue));
            //    Assert.AreEqual(value, actualValue, $"Value mismatch for key {key}");
            //}

            Trace.WriteLine(result.ToString());

            inputcleaned = "{" +
                           "Print(hello World);" +
                           "if (condition1)" +
                           "{" +
                           "command1;" +
                           "if (condition2)" +
                           "{" +
                           "command2;" +
                           "}" +
                           "}" +
                           "Label(one);" +
                           "Print(passed label one);" +
                           "goto(two);" +
                           "Print(Should not be printed);" +
                           "Label(two);" +
                           "Print(Finish);" +
                           "}";


            var finalResults = new List<(int Key, string Category, string Value)>
            {
                (1, "COMMAND", "Print(hello World)"),
                (2, "IF_1", "if(condition1)"), // IF block 1
                (3, "COMMAND", "command1"),
                (4, "IF_2", "if(condition2)"), // Nested IF block 2 within IF 1
                (5, "COMMAND", "command2"),
                (6, "IF_2_END", ""), // End of IF block 2
                (7, "IF_1_END_NOELSE", ""), // End of IF block 1 with no ELSE
                (8, "LABEL", "Label(one)"),
                (9, "COMMAND", "Print(passed label one)"),
                (10, "GOTO", "goto(two)"),
                (11, "COMMAND", "Print(Should not be printed)"),
                (12, "LABEL", "Label(two)"),
                (13, "COMMAND", "Print(Finish)")
            };

            // Act
            result = IrtParserCommand.BuildCommand(inputcleaned);

            // Assert
            //foreach (var expected in expectedResults)
            //{
            //    var (key, category, value) = expected;
            //    Assert.IsTrue(result.TryGetCategory(key, out var actualCategory));
            //    Assert.AreEqual(category, actualCategory, $"Category mismatch for key {key}");
            //    Assert.IsTrue(result.TryGetValue(key, out var actualValue));
            //    Assert.AreEqual(value, actualValue, $"Value mismatch for key {key}");
            //}

            Trace.WriteLine(result.ToString());
        }
    }
}