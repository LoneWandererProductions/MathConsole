/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/Interpret.cs
 * PURPOSE:     Tests for Interpreter
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterpreteTests
{
    /// <summary>
    ///     Interpreter unit test class.
    /// </summary>
    [TestClass]
    public sealed class Interpret
    {
        /// <summary>
        ///     The UserSpaceOne (const). Value: "NameSpaceOne".
        /// </summary>
        private const string UserSpaceOne = "NameSpaceOne";

        /// <summary>
        ///     The UserSpaceTwo (const). Value: "NameSpaceTwo".
        /// </summary>
        private const string UserSpaceTwo = "NameSpaceTwo";

        /// <summary>
        ///     The prompt.
        /// </summary>
        private static Prompt _prompt;

        /// <summary>
        ///     The out command.
        /// </summary>
        private static OutCommand _outCommand;

        /// <summary>
        ///     The log.
        /// </summary>
        private static string _log;

        /// <summary>
        ///     The batch
        /// </summary>
        private static readonly string Batch = Path.Combine(Directory.GetCurrentDirectory(), "MyTest.txt");

        /// <summary>
        ///     The Command Dictionary
        /// </summary>
        private static readonly Dictionary<int, InCommand> DctCommandOne = new()
        {
            { 0, new InCommand { Command = "com1", ParameterCount = 2, Description = "Help com1" } },
            { 1, new InCommand { Command = "com2", ParameterCount = 3, Description = "Help com2" } },
            {
                2,
                new InCommand { Command = "com3", ParameterCount = 0, Description = "Special case no Parameter" }
            }
        };

        /// <summary>
        ///     The alternate Command Dictionary
        /// </summary>
        private static readonly Dictionary<int, InCommand> DctCommandTwo = new()
        {
            { 4, new InCommand { Command = "Test", ParameterCount = 0, Description = "Here we go" } }
        };

        /// <summary>
        ///     The commands
        /// </summary>
        private Dictionary<int, InCommand> _commands;

        /// <summary>
        ///     The irt prompt
        /// </summary>
        private IrtParser _irtPrompt;

        /// <summary>
        ///     The namespace
        /// </summary>
        private string _namespace;

        /// <summary>
        ///     Sets up.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _irtPrompt = new IrtParser(new Prompt());
            _commands = new Dictionary<int, InCommand>
            {
                { 1, new InCommand { Command = "COMMAND1", ParameterCount = 0, Description = "Description1" } },
                { 2, new InCommand { Command = "COMMAND2", ParameterCount = 1, Description = "Description2" } }
            };
            _namespace = "TestNamespace";
            var userSpace = new UserSpace { Commands = _commands, UserSpaceName = _namespace };
            _irtPrompt.Initiate(userSpace);
        }

        /// <summary>
        ///     Check our Interpreter
        /// </summary>
        [TestMethod]
        public void ConsoleInterpreter()
        {
            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(DctCommandOne, UserSpaceOne);
            _prompt.ConsoleInput("coM1(1,2)");

            Assert.AreEqual(0, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            _prompt.ConsoleInput("com3()");

            Assert.AreEqual(2, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            _prompt.ConsoleInput("cOm3");

            Assert.AreEqual(2, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            _prompt.ConsoleInput("helP");

            Assert.IsTrue(_log.Contains("Basic Syntax"), "Wrong Basic Help: " + _log);

            _prompt.ConsoleInput("helP()");

            Assert.AreEqual(true,
                _log.Contains("Basic prompt, Version : 0.3. Author: Peter Geinitz (Wayfarer), not context sensitive"),
                "Help not displayed" + _log);

            _prompt.ConsoleInput("helP(CoM1)");

            Assert.AreEqual(true, _log.Contains("com1 Description"), "Wrong Help: " + _log);

            _prompt.Dispose();
        }

        /// <summary>
        ///     Check our Interpreter with wrong input
        /// </summary>
        [TestMethod]
        public void BreakConsoleInterpreter()
        {
            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(DctCommandOne, UserSpaceOne);
            _prompt.ConsoleInput("coM1(1)");

            Assert.IsTrue(_log.Contains("Error in the Syntax"), "Syntax Error: " + _log);
            Assert.AreEqual(-1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            _prompt.ConsoleInput("com1(1,2)");

            //Overload

            Assert.AreEqual(0, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual(2, _outCommand.Parameter.Count, "Wrong Number: " + _outCommand.Parameter.Count);

            _prompt.ConsoleInput(string.Empty);

            Assert.IsTrue(_log.Contains("Input was null or empty"), "Syntax Error: " + _log);
            Assert.AreEqual(-1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            _prompt.ConsoleInput("coM1(1,2");

            Assert.IsTrue(_log.Contains("Wrong parenthesis"), "Parenthesis Error: " + _log);
            Assert.AreEqual(-1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            //possible Break

            _prompt.ConsoleInput("com3");

            Assert.IsTrue(_log.Contains("Wrong parenthesis"), "Parenthesis Error: " + _log);

            //Internal Test

            _prompt.ConsoleInput("help");

            Assert.AreNotEqual("Error in the Syntax", _log, "Parenthesis Error: " + _log);

            _prompt.ConsoleInput("help()");

            Assert.AreEqual(true,
                _log.Contains("Basic prompt, Version : 0.3. Author: Peter Geinitz (Wayfarer), not context sensitive"),
                "Help not displayed" + _log);
            _prompt.Dispose();
        }

        /// <summary>
        ///     Some basic tests, the interpreter should execute commands correctly.
        /// </summary>
        [TestMethod]
        public void ConsoleInterpreterExecuteCommands()
        {
            // Arrange
            var prompt = new Prompt();
            prompt.SendLogs += SendLogs;
            prompt.SendCommands += SendCommands;
            var commands = new Dictionary<int, InCommand>
            {
                { 0, new InCommand { Command = "com1", ParameterCount = 2, Description = "Help com1" } },
                { 1, new InCommand { Command = "com2", ParameterCount = 3, Description = "Help com2" } },
                {
                    2,
                    new InCommand { Command = "com3", ParameterCount = 0, Description = "Special case no Parameter" }
                },
                {
                    3,
                    new InCommand
                        { Command = "com4", ParameterCount = -2, Description = "Special case 2+ parameters" }
                }
            };
            prompt.Initiate(commands, "NameSpaceOne");

            // Act
            prompt.ConsoleInput("com1(1,2)");

            // Assert
            Assert.AreEqual(0, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual(2, _outCommand.Parameter.Count, "Wrong Number: " + _outCommand.Parameter.Count);

            // Act
            prompt.ConsoleInput("com3(1,2)");

            // Assert
            Assert.AreEqual(-1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual(null, _outCommand.Parameter, "Wrong Number: ");
            Assert.IsTrue(_outCommand.ErrorMessage.Contains("Error in the Syntax: "),
                "Wrong or no error set: " + _outCommand.ErrorMessage);

            // Act
            prompt.ConsoleInput("help (CoM3 )");
            Assert.IsTrue(_log.Contains("Special case no Parameter"), "Help not correctly displayed.");

            // Act
            prompt.ConsoleInput("com3");

            // Assert
            Assert.AreEqual(2, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual(0, _outCommand.Parameter.Count, "Wrong Number: " + _outCommand.Parameter.Count);

            // Act
            prompt.ConsoleInput("com4(2,2)");

            // Assert
            Assert.AreEqual(3, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual(2, _outCommand.Parameter.Count, "Wrong Number: " + _outCommand.Parameter.Count);

            // Act
            prompt.ConsoleInput("com4(2,2,3)");

            // Assert
            Assert.AreEqual(3, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual(3, _outCommand.Parameter.Count, "Wrong Number: " + _outCommand.Parameter.Count);

            // Act
            prompt.ConsoleInput("com4(2)");

            // Assert
            Assert.AreEqual(-1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual(null, _outCommand.Parameter, "Wrong Number: ");
            Assert.IsTrue(_outCommand.ErrorMessage.Contains("Error in the Syntax: "),
                "Wrong or no error set: " + _outCommand.ErrorMessage);
        }

        /// <summary>
        ///     Check overload.
        /// </summary>
        [TestMethod]
        public void Overload()
        {
            var dct = new Dictionary<int, InCommand>();
            var command = new InCommand { Command = "com1", ParameterCount = 0, Description = "Help com1" };
            dct.Add(0, command);
            command = new InCommand { Command = "com1", ParameterCount = 1, Description = "Help com2" };
            dct.Add(1, command);

            //base

            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(dct, UserSpaceOne);
            _prompt.ConsoleInput("coM1()");

            Assert.AreEqual(0, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            //overload
            _prompt.ConsoleInput("coM1(1)");

            Assert.AreEqual(1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.AreEqual(1, _outCommand.Parameter.Count, "Wrong Number: " + _outCommand.Parameter.Count);

            //break
            _prompt.ConsoleInput("coM1(1,3)");

            Assert.AreEqual(-1, _outCommand.Command, "Wrong Id: " + _outCommand.Command);
            Assert.IsTrue(_log.Contains("Error in the Syntax"), "Syntax Error: " + _log);

            _prompt.Dispose();
        }

        /// <summary>
        ///     Check our Interpreter
        /// </summary>
        [TestMethod]
        public void ConsoleNameSpaceSwitch()
        {
            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(DctCommandOne, UserSpaceOne);
            _prompt.ConsoleInput("coM1(1,2)");

            Assert.AreEqual(0, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            _prompt.AddCommands(DctCommandTwo, UserSpaceTwo);

            Assert.AreEqual(2, _prompt.CollectedSpaces.Count, "Wrong Number of Namespaces");

            _prompt.ConsoleInput("use (" + UserSpaceTwo + ")");
            _prompt.ConsoleInput("test");

            Assert.AreEqual(4, _outCommand.Command, "Wrong Id: " + _outCommand.Command);

            _prompt.ConsoleInput("using");

            Assert.IsTrue(_log.StartsWith(UserSpaceTwo.ToUpper(), StringComparison.Ordinal), "Namespace not switched");
            _prompt.Dispose();
        }

        /// <summary>
        ///     Check our Interpreter
        /// </summary>
        [TestMethod]
        public void Container()
        {
            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(DctCommandOne, UserSpaceOne);

            _prompt.ConsoleInput("Container{ Help()};");

            Assert.AreEqual(true,
                _log.Contains("Basic prompt, Version : 0.3. Author: Peter Geinitz (Wayfarer)"),
                "Help not displayed" + _log);

            //advanced tests

            _prompt.ConsoleInput("Container{coM1(1,2); com3() ; ; --test comment;Help()};;;;");

            Assert.AreEqual(true,
                _log.Contains("Basic prompt, Version : 0.3. Author: Peter Geinitz (Wayfarer), not context sensitive"),
                "Help not displayed" + _log);

            Assert.AreEqual(_prompt.Log[3], "0", "Correct Parameter");
            Assert.AreEqual(_prompt.Log[5], "2", "Correct Parameter");
            _prompt.Dispose();
        }

        /// <summary>
        ///     Execute some Batch File
        /// </summary>
        [TestMethod]
        public void BatchCommand()
        {
            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(DctCommandOne, UserSpaceOne);

            CreateFile();

            _prompt.ConsoleInput("BATCHEXECUTE(" + Batch + ")");

            Assert.AreEqual(true,
                _log.Contains("Basic prompt, Version : 0.3. Author: Peter Geinitz (Wayfarer), not context sensitive"),
                "Help not displayed" + _log);

            Assert.AreEqual(_prompt.Log[1], "0", "Correct Parameter");
            Assert.AreEqual(_prompt.Log[2], "com3()", "Correct Parameter");
            Assert.AreEqual(_prompt.Log[3], "2", "Correct Parameter");

            if (File.Exists(Batch)) File.Delete(Batch);

            _prompt.Dispose();
        }

        /// <summary>
        ///     Jumps the command.
        /// </summary>
        [TestMethod]
        public void JumpCommand()
        {
            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(DctCommandOne, UserSpaceOne);

            var command = "Container{ " +
                          "Print(hello World);" +
                          "Label(one);" +
                          "Print(passed label one);" +
                          "goto(two);" +
                          "Print(Should not be printed);" +
                          "Label(two);" +
                          "Print(Finish);" +
                          "};";

            _prompt.ConsoleInput(command);

            Assert.AreEqual(true,
                _log.Contains("Finish", StringComparison.CurrentCultureIgnoreCase),
                "Not correctly jumped" + _log);


            command = "Container{ " +
                      "Print(hello World);" +
                      "Label(one);" +
                      "Print(passed label one);" +
                      "goto(two);" +
                      "Print(Should not be printed);" +
                      "Label(two);" +
                      "Print(Finish);" +
                      "goto(three);" +
                      "};";

            _prompt.ConsoleInput(command);

            Assert.AreEqual(true,
                _log.Contains("error jump label not found:", StringComparison.CurrentCultureIgnoreCase),
                "Not correctly jumped" + _log);

            //only the stuff in command should be used
            command = "Container{Print  (hello World)  } ; Print ( not hello )";

            _prompt.ConsoleInput(command);
            Assert.AreEqual(true,
                _log.Contains("hello World", StringComparison.CurrentCultureIgnoreCase),
                "Not correctly jumped" + _log);

            _prompt.Dispose();
        }


        /// <summary>
        ///     Creates the file.
        /// </summary>
        private static void CreateFile()
        {
            if (File.Exists(Batch)) File.Delete(Batch);

            // Create a file to write to.
            using var sw = File.CreateText(Batch);
            sw.WriteLine("coM1(1,2);");
            sw.WriteLine("com3();");
            sw.WriteLine("Help();");
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