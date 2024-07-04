/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtPrompt.cs
 * PURPOSE:     Handle the Input
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Interpreter
{
    /// <summary>
    ///     Basic command Line Interpreter, bare bones for now
    /// </summary>
    internal sealed class IrtPrompt : IDisposable
    {
        /// <summary>
        ///     Command Register
        /// </summary>
        private static Dictionary<int, InCommand> _com;

        /// <summary>
        ///     Namespace of Commands
        /// </summary>
        private static string _nameSpace;

        /// <summary>
        ///     Extension Command Register
        /// </summary>
        private static Dictionary<int, InCommand> _extension;

        /// <summary>
        ///     The prompt
        /// </summary>
        private Prompt _prompt;

        /// <summary>
        ///     The original input string
        /// </summary>
        private string _inputString;

        /// <summary>
        ///     The IRT extension
        /// </summary>
        private IrtExtension _irtExtension;

        /// <summary>
        ///     The IRT internal
        /// </summary>
        private IrtInternal _irtInternal;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IrtPrompt" /> class.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        public IrtPrompt(Prompt prompt)
        {
            _prompt = prompt;
        }

        /// <summary>
        ///     Event to send selected command to the subscriber
        /// </summary>
        internal event EventHandler<string> SendInternalLog;

        /// <summary>
        ///     Get the engine running
        /// </summary>
        /// <param name="use">The command structure</param>
        internal void Initiate(UserSpace use)
        {
            // Set the basic parameters
            _com = use.Commands;
            _extension = use.ExtensionCommands;
            _nameSpace = use.UserSpaceName;
            _irtInternal = new IrtInternal(use.Commands, use.UserSpaceName, _prompt);
            _irtExtension = new IrtExtension();

            // Notify log about loading up
            var log = Logging.SetLastError(IrtConst.InformationStartup, 2);
            SendInternalLog?.Invoke(this, log);
        }

        /// <summary>
        ///     Switches the user space.
        /// </summary>
        /// <param name="use">The use.</param>
        internal static void SwitchUserSpace(UserSpace use)
        {
            _com = use.Commands;
            _extension = use.ExtensionCommands;
            _nameSpace = use.UserSpaceName;
        }

        /// <summary>
        ///     Handles the input string and processes commands.
        /// </summary>
        /// <param name="inputString">Input string</param>
        internal void HandleInput(string inputString)
        {
            _inputString = inputString;

            if (string.IsNullOrWhiteSpace(inputString))
            {
                SetError(IrtConst.ErrorInvalidInput);
                return;
            }

            // Validate the input string
            if (!CleanInputString(ref inputString))
            {
                SetError(Logging.SetLastError(IrtConst.ParenthesisError, 0));
                return;
            }

            // Handle comment commands
            if (IsCommentCommand(inputString))
            {
                Trace.WriteLine(inputString);
                return;
            }

            // Handle help commands
            if (IsHelpCommand(inputString))
            {
                _prompt.SendLog(this, IrtConst.HelpGeneric);
                return;
            }

            // Check if the parentheses are correct
            if (!Irt.ValidateParameters(inputString))
            {
                SetErrorWithLog(IrtConst.ParenthesisError);
                return;
            }

            // Check for extensions in the internal namespace first, then in the external namespace if needed
            var extensionResult = _irtExtension.CheckForExtension(_inputString, IrtConst.InternalNameSpace, IrtConst.InternalExtensionCommands);

            if (extensionResult.Status == IrtConst.Error)
                extensionResult = _irtExtension.CheckForExtension(_inputString, _nameSpace, _extension);

            // Process the extension result
            switch (extensionResult.Status)
            {
                case IrtConst.Error:
                    SetError(Logging.SetLastError($"{IrtConst.ErrorExtensions}{IrtConst.Error}", 0));
                    return;

                case IrtConst.ParameterMismatch:
                    SetError(Logging.SetLastError($"{IrtConst.ErrorExtensions}{IrtConst.ParameterMismatch}", 0));
                    return;

                case IrtConst.ExtensionFound:
                    if (extensionResult.Extension.ExtensionNameSpace == IrtConst.InternalNameSpace)
                    {
                        ProcessExtensionInternal(extensionResult.Extension);
                    }
                    else
                    {
                        var command = ProcessInput(inputString, extensionResult.Extension);
                        SetResult(command);
                    }
                    return;

                default:
                    var com = ProcessInput(inputString);
                    if (com != null) SetResult(com);
                    break;
            }
        }

        /// <summary>
        ///     Processes the internal extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        private void ProcessExtensionInternal(ExtensionCommands extension)
        {
            switch (extension.ExtensionCommand)
            {
                case 0:
                    // Switch namespace
                    _prompt.SwitchNameSpaces(extension.ExtensionParameter[0]);
                    _prompt.ConsoleInput(extension.BaseCommand);
                    break;

                case 1:
                    var key = Irt.CheckForKeyWord(extension.BaseCommand, IrtConst.InternCommands);
                    if (key != IrtConst.Error)
                    {
                        var command = IrtConst.InternCommands[key];

                        using (var irtInternal = new IrtInternal(IrtConst.InternCommands, IrtConst.InternalNameSpace, _prompt))
                        {
                            irtInternal.ProcessInput(IrtConst.InternalHelpWithParameter, command.Command);
                        }

                        _prompt.CommandRegister = new IrtFeedback
                        {
                            AwaitedInput = -1,
                            AwaitInput = true,
                            IsInternalCommand = true,
                            InternalInput = extension.BaseCommand,
                            CommandHandler = _irtInternal,
                            Key = key
                        };
                    }
                    else
                    {
                        var com = ProcessInput(extension.BaseCommand);
                        var command = _com[com.Command];
                        _irtInternal.ProcessInput(IrtConst.InternalHelpWithParameter, command.Command);

                        _prompt.CommandRegister = new IrtFeedback
                        {
                            AwaitedInput = -1,
                            AwaitInput = true,
                            AwaitedOutput = com
                        };
                    }
                    break;

                default:
                    _prompt.SendLogs(this, "Unknown extension command.");
                    break;
            }
        }

        /// <summary>
        ///     Processes the input string.
        /// </summary>
        /// <param name="inputString">Input string</param>
        /// <param name="extension">Optional extension commands</param>
        private OutCommand ProcessInput(string inputString, ExtensionCommands extension = null)
        {
            if (_com == null)
            {
                SetError(IrtConst.ErrorNoCommandsProvided);
                return null;
            }

            var key = Irt.CheckForKeyWord(inputString, IrtConst.InternCommands);
            if (key != IrtConst.Error)
            {
                _irtInternal.ProcessInput(key, inputString);
                return null;
            }

            key = Irt.CheckForKeyWord(inputString, _com);
            if (key == IrtConst.Error)
            {
                SetErrorWithLog(IrtConst.KeyWordNotFoundError, _inputString);
                return null;
            }

            var (status, splitParameter) = Irt.ProcessParameters(inputString, key, _com);
            var parameter = status == IrtConst.ParameterCommand
                ? Irt.SplitParameter(splitParameter, IrtConst.Splitter)
                : new List<string> { splitParameter };

            var check = Irt.CheckOverload(_com[key].Command, parameter.Count, _com);
            if (check == null)
            {
                SetErrorWithLog(IrtConst.SyntaxError);
                return null;
            }

            return new OutCommand
            {
                Command = (int)check,
                Parameter = parameter,
                UsedNameSpace = _nameSpace,
                ExtensionCommand = extension
            };
        }

        /// <summary>
        ///     Cleans the input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>True if input string is cleaned and valid; otherwise false.</returns>
        private static bool CleanInputString(ref string input)
        {
            input = Irt.WellFormedParenthesis(input).ToUpper(CultureInfo.InvariantCulture);
            var openParenthesis = new[] { IrtConst.BaseOpen, IrtConst.AdvancedOpen };
            var closeParenthesis = new[] { IrtConst.BaseClose, IrtConst.AdvancedClose };

            return Irt.CheckMultiple(input, openParenthesis, closeParenthesis);
        }

        /// <summary>
        ///     Determines whether the specified input is a comment command.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///     <c>true</c> if the specified input is a comment command; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCommentCommand(string input)
        {
            return input.StartsWith(IrtConst.CommentCommand, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Determines whether the specified input is a help command.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///     <c>true</c> if the specified input is a help command; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsHelpCommand(string input)
        {
            input = input.Replace(IrtConst.BaseOpen.ToString(), string.Empty)
                         .Replace(IrtConst.BaseClose.ToString(), string.Empty);
            return input.Equals(IrtConst.InternalCommandHelp, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Sets the error with log.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="additionalInfo">Additional information.</param>
        private void SetErrorWithLog(string errorMessage, string additionalInfo = "")
        {
            var log = Logging.SetLastError(errorMessage, 0);
            SetError(string.Concat(log, additionalInfo));
        }

        /// <summary>
        ///     Sets the result of a command.
        /// </summary>
        /// <param name="command">The command.</param>
        private void SetResult(OutCommand command)
        {
            if (_com[command.Command].FeedbackId == 0)
                _prompt.SendCommand(this, command);
            else
                _prompt.SetFeedbackLoop(_com[command.Command].FeedbackId, command);
        }

        /// <summary>
        ///     Sets the error status of the output command.
        /// </summary>
        /// <param name="error">The error message.</param>
        private void SetError(string error)
        {
            var com = new OutCommand
            {
                Command = IrtConst.Error,
                Parameter = null,
                UsedNameSpace = _nameSpace,
                ErrorMessage = error
            };

            _prompt.SendCommand(this, com);
        }

        private bool _disposed = false;

        /// <inheritdoc />
        /// <summary>
        ///     Dispose of the resources used by the IrtPrompt.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose the resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (true) or from a finalizer (false).</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources here if needed
                _irtInternal?.Dispose();
            }

            // Dispose unmanaged resources here if needed

            _disposed = true;
        }

        /// <summary>
        ///     Destructor to ensure the resources are released.
        /// </summary>
        ~IrtPrompt()
        {
            Dispose(false);
        }
    }
}
