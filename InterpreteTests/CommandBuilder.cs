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

            var result = IrtIfElseParser.BuildCommand(inputcleaned);
        }
    }
}