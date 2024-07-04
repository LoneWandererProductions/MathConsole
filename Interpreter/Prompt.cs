/*
 * COPYRIGHT:   See COPYING in the top-level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/Prompt.cs
 * PURPOSE:     Handle the Input, Entry point for all Commands
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExtendedSystemObjects;

namespace Interpreter
{
    /// <inheritdoc cref="IPrompt" />
    /// <summary>
    ///     Bucket List:
    ///     - Overloads
    ///     - Nested Commands
    /// </summary>
    /// <seealso cref="T:Interpreter.IPrompt" />
    /// <seealso cref="T:System.IDisposable" />
    public sealed class Prompt : IPrompt, IDisposable
    {
        /// <summary>
        ///     Used to interpret commands
        /// </summary>
        private static IrtPrompt _interpret;

        /// <summary>
        ///     The count
        /// </summary>
        private static int _count = -1;

        /// <summary>
        ///     The feedback handler
        /// </summary>
        private IrtHandleFeedback _feedbackHandler;

        /// <summary>
        ///     User input window
        /// </summary>
        private WindowPrompt _prompt;

        /// <summary>
        ///     Gets or sets the command register.
        /// </summary>
        internal IrtFeedback CommandRegister { get; set; }

        /// <summary>
        ///     The collected namespaces
        /// </summary>
        public Dictionary<string, UserSpace> CollectedSpaces { get; private set; }

        /// <summary>
        ///     Sends selected command to the subscriber
        /// </summary>
        public EventHandler<OutCommand> SendCommands { get; set; }

        /// <summary>
        ///     Sends message to the subscriber
        /// </summary>
        public EventHandler<string> SendLogs { get; set; }

        /// <summary>
        ///     Gets the log.
        /// </summary>
        public Dictionary<int, string> Log { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Prompt" /> is disposed.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        ///     Maximum number of lines in the log
        /// </summary>
        private static int MaxLines => 1000;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Prompt" /> class.
        /// </summary>
        public Prompt()
        {
            ResetState();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes of the resources used by the <see cref="Prompt" /> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (true) or from a finalizer (false).</param>
        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                // Dispose managed resources
                _interpret = null;
                CollectedSpaces = null;
                Log = null;
                _prompt?.Close();
                _prompt = null;
                _feedbackHandler = null;
                CommandRegister = null;
            }

            // Note: If there are unmanaged resources, they should be released here.

            Disposed = true;
        }

        /// <summary>
        ///     Destructor to ensure resources are released.
        /// </summary>
        ~Prompt()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Start the sender and interpreter.
        ///     UserSpace will never overwrite the system commands.
        ///     Parentheses are not defined by the UserSpace but by the definition of the User, so both ( or [ can be valid.
        /// </summary>
        /// <param name="com">Command register</param>
        /// <param name="userSpace">UserSpace of the register</param>
        /// <param name="extension">Optional extension methods</param>
        /// <param name="userFeedback">Optional user feedback methods</param>
        public void Initiate(Dictionary<int, InCommand> com, string userSpace,
            Dictionary<int, InCommand> extension = null, Dictionary<int, UserFeedback> userFeedback = null)
        {
            ResetState();
            CommandRegister = new IrtFeedback();
            var use = new UserSpace { UserSpaceName = userSpace, Commands = com, ExtensionCommands = extension };
            _feedbackHandler = new IrtHandleFeedback(this, userFeedback, use);

            // Upper is needed because of the way we compare commands in the interpreter
            CollectedSpaces.AddDistinct(userSpace.ToUpper(), use);

            _interpret = new IrtPrompt(this);
            _interpret.Initiate(use);
            _interpret.SendInternalLog += SendLog;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adds commands to the interpreter.
        /// </summary>
        /// <param name="com">Command register</param>
        /// <param name="userSpace">UserSpace of the register</param>
        /// <param name="extension">Optional extension methods</param>
        public void AddCommands(Dictionary<int, InCommand> com, string userSpace,
            Dictionary<int, InCommand> extension = null)
        {
            if (CollectedSpaces.IsNullOrEmpty())
            {
                SendLogs?.Invoke(this, IrtConst.ErrorNotInitialized);
                return;
            }

            var use = CreateUserSpace(userSpace, com);
            CollectedSpaces.AddDistinct(userSpace.ToUpper(), use);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Starts a window for user input.
        /// </summary>
        public void StartWindow()
        {
            _prompt = new WindowPrompt(_interpret) { ShowInTaskbar = true };
            _prompt.Show();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Handles console input.
        /// </summary>
        /// <param name="input">Input string</param>
        public void ConsoleInput(string input)
        {
            if (!CommandRegister.AwaitInput) _interpret?.HandleInput(input);
            else _feedbackHandler.HandleUserInput(input);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Handles callback for window feedback.
        /// </summary>
        /// <param name="message">Feedback message for display</param>
        public void CallbacksWindow(string message)
        {
            _prompt?.FeedbackMessage(message);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Handles generic callback from outside.
        /// </summary>
        /// <param name="message">Feedback message for display</param>
        public void Callback(string message)
        {
            SendLogs?.Invoke(nameof(Callback), message);
        }

        /// <summary>
        ///     Switches the namespaces.
        /// </summary>
        /// <param name="space">The namespace to use</param>
        internal void SwitchNameSpaces(string space)
        {
            space = space.ToUpper(CultureInfo.InvariantCulture);
            var use = CollectedSpaces[space];
            IrtPrompt.SwitchUserSpace(use);
            // Switch UserSpace here as well
            _feedbackHandler.Use = use;
        }

        /// <summary>
        ///     Sets up the feedback loop.
        /// </summary>
        /// <param name="feedbackId">The feedback identifier</param>
        /// <param name="com">The command</param>
        internal void SetFeedbackLoop(int feedbackId, OutCommand com)
        {
            CommandRegister = new IrtFeedback
            {
                AwaitedOutput = com,
                AwaitInput = true,
                AwaitedInput = feedbackId
            };
        }

        /// <summary>
        ///     Returns the selected command.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event arguments</param>
        internal void SendCommand(object sender, OutCommand e)
        {
            AddToLog(e.Command == IrtConst.Error ? e.ErrorMessage : e.Command.ToString());
            SendCommands?.Invoke(nameof(Prompt), e);
        }

        /// <summary>
        ///     Sends log messages.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event arguments</param>
        internal void SendLog(object sender, string e)
        {
            AddToLog(e);
            SendLogs?.Invoke(nameof(Prompt), e);
        }

        /// <summary>
        ///     Adds a message to the log.
        /// </summary>
        /// <param name="message">The message</param>
        internal void AddToLog(string message)
        {
            if (_count == MaxLines) Log.Remove(Log.Keys.First());

            _count++;
            Log.Add(_count, message);
        }

        /// <summary>
        ///     Resets the state of the instance.
        /// </summary>
        private void ResetState()
        {
            CollectedSpaces = new Dictionary<string, UserSpace>();
            Log = new Dictionary<int, string>();
            _count = -1;
        }

        /// <summary>
        ///     Creates a new UserSpace.
        /// </summary>
        /// <param name="userSpace">The user space</param>
        /// <param name="com">The command register</param>
        /// <returns>The created UserSpace</returns>
        private UserSpace CreateUserSpace(string userSpace, Dictionary<int, InCommand> com)
        {
            var use = new UserSpace { UserSpaceName = userSpace, Commands = com };
            CollectedSpaces.AddDistinct(userSpace.ToUpper(), use);
            return use;
        }
    }
}

