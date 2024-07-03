/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/FeedbackTests.cs
 * PURPOSE:     Tests for the Feedback loop
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Diagnostics;
using Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterpreteTests
{
    /// <summary>
    ///     Test user Feedback
    /// </summary>
    [TestClass]
    public sealed class FeedbackTests
    {
        /// <summary>
        ///     The log
        /// </summary>
        private static string _log;

        /// <summary>
        ///     The out command
        /// </summary>
        private static OutCommand _outCommand;

        /// <summary>
        ///     The prompt
        /// </summary>
        private readonly Prompt _prompt = new();

        /// <summary>
        ///     The user feedback
        /// </summary>
        private readonly Dictionary<int, UserFeedback> _userFeedback = new();

        /// <summary>
        ///     Handles the user input valid input show initial message and send commands.
        /// </summary>
        [TestMethod]
        public void HandleUserInputValidInputShowInitialMessageAndSendCommands()
        {
            // Arrange
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            var options = new Dictionary<AvailableFeedback, string> { { AvailableFeedback.Yes, "" } };

            _userFeedback[1] = new UserFeedback
            {
                Options = options
            };

            _prompt.CommandRegister = new IrtFeedback
            {
                AwaitedInput = 1,
                AwaitInput = true
            };

            var handleFeedback = new IrtHandleFeedback(_userFeedback, _prompt);

            // Act
            handleFeedback.HandleUserInput(" yES");

            // Assert
            Assert.IsTrue(_prompt.CommandRegister.InitialMessageShown);
            // Add more assertions based on expected behavior after handling input
        }

        /// <summary>
        ///     Handles the user input invalid awaited input no commands sent.
        /// </summary>
        [TestMethod]
        public void HandleUserInputInvalidAwaitedInputNoCommandsSent()
        {
            // Arrange
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;

            _prompt.CommandRegister = new IrtFeedback
            {
                AwaitedInput = 999, // Assuming this key doesn't exist in _userFeedback or IrtConst.InternalFeedback
                AwaitInput = true
            };

            var handleFeedback = new IrtHandleFeedback(_userFeedback, _prompt);

            // Act
            handleFeedback.HandleUserInput(" yES");

            // Assert
            Assert.IsFalse(_prompt.CommandRegister.AwaitInput);
            // Add more assertions based on expected behavior after handling input
        }

        /// <summary>
        ///     Handles the user input null feedback logs error.
        /// </summary>
        [TestMethod]
        public void HandleUserInputNullFeedbackLogsError()
        {
            // Arrange
            var prompt = new Prompt();
            prompt.SendLogs += SendLogs;
            prompt.SendCommands += SendCommands;

            prompt.CommandRegister = new IrtFeedback
            {
                AwaitedInput = 1, // Assuming this key doesn't exist in _userFeedback or IrtConst.InternalFeedback
                AwaitInput = true
            };

            var handleFeedback = new IrtHandleFeedback(_userFeedback, prompt);

            // Act
            handleFeedback.HandleUserInput(" yES");

            // Assert
            // Verify that an error command is sent or appropriate logging occurs
            // Example: Assert.AreEqual(expectedErrorMessage, _prompt.LastErrorMessage);
            Assert.AreEqual("No Options were provided.", _log, "No error Message send");
        }

        /// <summary>
        ///     Feedback and extension test.
        /// </summary>
        [TestMethod]
        public void FeedbackExtension()
        {
            // Arrange
            var dctCommandOne = new Dictionary<int, InCommand>
            {
                {
                    0, new InCommand { Command = "First", ParameterCount = 2, Description = "Help First" }
                },
                {
                    1,
                    new InCommand { Command = "Second", ParameterCount = 0, Description = "Second Command Namespace 1" }
                },
                {
                    2,
                    new InCommand { Command = "Third", ParameterCount = 0, Description = "Special case no Parameter" }
                }
            };

            // Act
            var prompt = new Prompt();
            prompt.SendLogs += SendLogs;
            prompt.SendCommands += SendCommands;
            prompt.Initiate(dctCommandOne, "UserSpace 1");
            prompt.ConsoleInput("FirSt(1,2).Help()");
            prompt.ConsoleInput("");


            Assert.IsFalse(_log.Contains("Help First"), "No help provided.");
            // Assert
            Assert.AreEqual("Option not allowed.", _log, "Error was not catched.");

            prompt.ConsoleInput("mehh");

            Assert.AreEqual("Option not allowed.", _log, "Error was not catched.");

            prompt.ConsoleInput(" yeS   ");

            Trace.WriteLine(_outCommand.ToString());

            Assert.IsNotNull(_outCommand, "Out Command was not empty.");
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
            if (e == null)
            {
                _log = string.Empty;
                _outCommand = null;
                return;
            }

            if (e.Command == -1) _log = e.ErrorMessage;
            _outCommand = e;
        }
    }
}