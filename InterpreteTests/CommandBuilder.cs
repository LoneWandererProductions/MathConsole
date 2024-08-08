using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterpreteTests
{
    [TestClass]
    public class CommandBuilder
    {
        /// <summary>
        ///     Parses the invalid input throws exception.
        /// </summary>
        [TestMethod]
        public void ParseComplexCommand()
        {
            //base, the command will be removed in the IrtParser
            var input = "Container{ " +
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
            var result = IrtIfElseParser.BuildCommand(inputcleaned);
            result = IrtIfElseParser.BuildCommand(inputcleaned);

            // Assert
            foreach (var expected in expectedResults)
            {
                var (key, category, value) = expected;
                Assert.IsTrue(result.TryGetCategory(key, out var actualCategory));
                Assert.AreEqual(category, actualCategory, $"Category mismatch for key {key}");
                Assert.IsTrue(result.TryGetValue(key, out var actualValue));
                Assert.AreEqual(value, actualValue, $"Value mismatch for key {key}");
            }

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

        }

        [TestMethod]
        public void TestParseIfElseClauses_SingleIfElse()
        {
            // Arrange
            string code = "if(condition1) { /* code */ } else { /* code */ }";

            // Act
            var clauses = IfElseParser2.ParseIfElseClauses(code);

            // Assert
            Assert.AreEqual(1, clauses.Count, "Expected 1 if-else clause.");
            Assert.AreEqual(0, clauses[0].IfIndex, "IfIndex should be 0.");
            Assert.AreEqual(20, clauses[0].ElseIndex, "ElseIndex should be 20.");
            Assert.AreEqual("if(condition1) { /* code */ } else { /* code */ }", clauses[0].Block);
        }

        [TestMethod]
        public void TestParseIfElseClauses_NestedIfElse()
        {
            // Arrange
            string code = "if(condition1) { if(condition2) { /* nested code */ } else { /* nested else code */ } } else { /* outer else code */ }";

            // Act
            var clauses = IfElseParser2.ParseIfElseClauses(code);

            // Assert
            Assert.AreEqual(2, clauses.Count, "Expected 2 if-else clauses.");

            // Check the first clause
            Assert.AreEqual(0, clauses[0].IfIndex, "IfIndex of the first clause should be 0.");
            Assert.IsTrue(clauses[0].Block.Contains("if(condition2)"), "Block should contain the nested if.");
            Assert.AreEqual(48, clauses[0].ElseIndex, "ElseIndex of the first clause should be 48.");

            // Check the second clause
            Assert.AreEqual(48, clauses[1].IfIndex, "IfIndex of the second clause should be 48.");
            Assert.AreEqual("else { /* outer else code */ }", clauses[1].Block);
            Assert.AreEqual(-1, clauses[1].ElseIndex, "The second clause should not have an outer else.");
        }
    }
}
