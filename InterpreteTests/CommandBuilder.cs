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
        /// Parses the invalid input throws exception.
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

            // Assert
            foreach (var expected in expectedResults)
            {
                var (key, category, value) = expected;
                Assert.IsTrue(result.TryGetCategory(key, out string actualCategory));
                Assert.AreEqual(category, actualCategory, $"Category mismatch for key {key}");
                Assert.IsTrue(result.TryGetValue(key, out string actualValue));
                Assert.AreEqual(value, actualValue, $"Value mismatch for key {key}");
            }


            Trace.WriteLine(result.ToString());
        }
    }
}