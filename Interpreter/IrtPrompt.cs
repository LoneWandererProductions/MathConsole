/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtPrompt.cs
 * PURPOSE:     Handle the Input
 * PROGRAMER:   Peter Geinitz (Wayfarer)
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
    internal sealed class IrtPrompt
    {
        /// <summary>
        ///     Command Register
        /// </summary>
        private static Dictionary<int, InCommand> _com;

        /// <summary>
        ///     Extension Command Register
        /// </summary>
        private static Dictionary<int, InCommand> _extension;

        /// <summary>
        ///     Namespace of Commands
        /// </summary>
        private static string _nameSpace;

        /// <summary>
        ///     The send logs
        /// </summary>
        private readonly EventHandler<string> _sendLogs;

        /// <summary>
        ///     Send selected Command to the Subscriber
        /// </summary>
        internal EventHandler<OutCommand> sendCommand;

        /// <summary>
        /// The prompt
        /// </summary>
        private readonly Prompt _prompt;

        /// <summary>
        ///     The log
        /// </summary>
        private readonly Dictionary<int, string> _log;

        /// <summary>
        ///     Send selected Command to the Subscriber
        /// </summary>
        internal EventHandler<string> SendInternalLog;

        /// <summary>
        /// The original input string
        /// </summary>
        private string _inputString;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IrtPrompt" /> class.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        public IrtPrompt(Prompt prompt)
        {
            _log = prompt.Log;
            _sendLogs = prompt.SendLogs;
            _prompt = prompt;
        }

        /// <summary>Get the Engine Running</summary>
        /// <param name="use">The Command Structure</param>
        internal void Initiate(UserSpace use)
        {
            _com = use.Commands;
            _extension = use.ExtensionCommands;
            _nameSpace = use.UserSpaceName;
            var log = Logging.SetLastError(IrtConst.InformationStartup, 2);
            OnStatus(log);
        }

        /// <summary>
        /// Switches the user space.
        /// </summary>
        /// <param name="use">The use.</param>
        internal static void SwitchUserSpace(UserSpace use)
        {
            _com = use.Commands;
            _extension = use.ExtensionCommands;
            _nameSpace = use.UserSpaceName;
        }

        /// <summary>
        ///     will do all the work
        /// </summary>
        /// <param name="inputString">Input string</param>
        /// <returns>Results of our commands</returns>
        internal void HandleInput(string inputString)
        {
            // Save a copy for debugging and logging reasons
            _inputString = inputString;

            // Clean the input string
            inputString = CleanInputString(inputString);

            // Check for extension methods
            var extensionResult = Irt.CheckForExtension(_inputString, _nameSpace, _extension);

            // Handle file comments
            if (IsCommentCommand(inputString))
            {
                Trace.WriteLine(inputString);
                return;
            }

            // Handle help command
            if (IsHelpCommand(inputString))
            {
                OnStatus(IrtConst.HelpGeneric);
                return;
            }

            // Handle internal commands
            var param = Irt.CheckInternalCommands(inputString, IrtConst.InternalCommands);
            if (!string.IsNullOrEmpty(param))
            {
                HandleInternalCommands(param, inputString);
                return;
            }

            // Check if command dictionary is empty
            if (_com == null)
            {
                SetError(IrtConst.ErrorNoCommandsProvided);
                return;
            }

            // Get the command key from the dictionary
            var key = Irt.CheckForKeyWord(inputString, _com);

            // Handle case where key is not found
            if (key == IrtConst.ErrorParam)
            {
                SetErrorWithLog(IrtConst.KeyWordNotFoundError, _inputString);
                return;
            }

            // Validate parameter count and parentheses
            if (!ValidateParameters(inputString, key))
            {
                return;
            }

            // Process parameters and handle overloads
            var parameterPart = ProcessParameters(inputString, key);
            var parameter = Irt.SplitParameter(parameterPart, IrtConst.Splitter);
            var check = Irt.CheckOverload(_com[key].Command, parameter.Count, _com);

            if (check == null)
            {
                SetErrorWithLog(IrtConst.SyntaxError);
                return;
            }

            SetResult((int)check, parameter);
        }

        /// <summary>
        /// Cleans the input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static string CleanInputString(string input)
        {
            return input.Trim().ToUpper(CultureInfo.CurrentCulture).ToUpper(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Determines whether [is comment command] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if [is comment command] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCommentCommand(string input)
        {
            return input.StartsWith(IrtConst.CommentCommand, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether [is help command] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if [is help command] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsHelpCommand(string input)
        {
            return input.Equals(IrtConst.InternalCommandHelp, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Sets the error with log.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="additionalInfo">The additional information.</param>
        private void SetErrorWithLog(string errorMessage, string additionalInfo = "")
        {
            var log = Logging.SetLastError(errorMessage, 0);
            SetError(string.Concat(log, additionalInfo));
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns>Check if Parameter is valid</returns>
        private bool ValidateParameters(string input, int key)
        {
            if (_com[key].ParameterCount == 0 || Irt.SingleCheck(input)) return true;

            SetErrorWithLog(IrtConst.ParenthesisError);
            return false;
        }

        /// <summary>
        /// Processes the parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns>return Parameter</returns>
        private static string ProcessParameters(string input, int key)
        {
            var command = _com[key].Command.ToUpper(CultureInfo.InvariantCulture);
            var parameterPart = Irt.RemoveWord(command, input);
            return Irt.RemoveParenthesis(parameterPart, IrtConst.BaseClose, IrtConst.BaseOpen);
        }

        /// <summary>
        ///     For Internal Commands
        /// </summary>
        /// <param name="param">Parameter of the internal Command.</param>
        /// <param name="inputString">Input string</param>
        private void HandleInternalCommands(string param, string inputString)
        {
            //sure the beginning is like the Internal command but with some extras we do not like
            if (param == IrtConst.InternalCommandList && param != inputString)
            {
                var log = Logging.SetLastError(IrtConst.SyntaxError, 0);
                SetError(log);
                return;
            }

            var parameterPart = Irt.RemoveWord(param.ToUpper(CultureInfo.InvariantCulture), inputString);

            switch (param)
            {
                case IrtConst.InternalCommandHelp:
                    CommandHelp(parameterPart);
                    break;

                case IrtConst.InternalCommandList:
                    CommandList();
                    break;

                case IrtConst.InternalUsing:
                    CommandUsing(_nameSpace);
                    break;

                case IrtConst.InternalUse:
                    CommandUse(parameterPart);
                    break;

                case IrtConst.InternalErrorLog:
                    CommandLogError();
                    break;

                case IrtConst.InternalLogInfo:
                    CommandLogInfo();
                    break;

                case IrtConst.InternalLogFull:
                    CommandLogFull();
                    break;

                case IrtConst.InternalCommandContainer:
                    CommandContainer(inputString, parameterPart);
                    break;

                case IrtConst.InternalCommandBatchExecute:
                    CommandBatchExecute(parameterPart);
                    break;

                default:
                    OnStatus(string.Concat(IrtConst.KeyWordNotFoundError, param));
                    break;
            }
        }

        /// <summary>
        ///     Return help for specific command
        /// </summary>
        /// <param name="parameterPart">Parameter about what we want help about</param>
        private void CommandHelp(string parameterPart)
        {
            //Empty parameters
            if (parameterPart == IrtConst.InternalEmptyParameter)
            {
                OnStatus(IrtConst.HelpGeneric);
                return;
            }

            //Remove Parenthesis
            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseClose, IrtConst.BaseOpen);

            //Get the Id
            var key = Irt.CheckForKeyWord(parameterPart, _com);

            //anything else error out
            if (key == IrtConst.ErrorParam)
            {
                var log = Logging.SetLastError(string.Concat(IrtConst.KeyWordNotFoundError, parameterPart), 0);
                SetError(log);
                return;
            }

            OnStatus(string.Concat(_com[key].Command, IrtConst.FormatDescription, _com[key].Description,
                IrtConst.FormatCount,
                _com[key].ParameterCount));
        }

        /// <summary>Command to switch between using.</summary>
        /// <param name="parameterPart">The parameter part.</param>
        private void CommandUse(string parameterPart)
        {
            //Remove Parenthesis and split
            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseClose, IrtConst.BaseOpen);

            if (!_prompt.CollectedSpaces.ContainsKey(parameterPart))
            {
                OnStatus(IrtConst.ErrorUserSpaceNotFound);
                return;
            }

            _prompt.SwitchNameSpaces(parameterPart);
            var log = string.Concat(IrtConst.InformationNamespaceSwitch, parameterPart);
            OnStatus(log);

        }

        /// <summary>
        ///     List all available Commands
        /// </summary>
        private void CommandList()
        {
            foreach (var com in _com.Values)
            {
                OnStatus(string.Concat(com.Command, Environment.NewLine, com.Description));
            }
        }

        /// <summary>Display all using and the Current in Use.</summary>
        /// <param name="nameSpace">The name space.</param>
        private void CommandUsing(string nameSpace)
        {
            OnStatus(string.Concat(IrtConst.Active, nameSpace, Environment.NewLine));
            foreach (var key in _prompt.CollectedSpaces.Keys)
            {
                OnStatus(string.Concat(key, Environment.NewLine));
            }
        }

        /// <summary>
        ///     Commands the log.
        /// </summary>
        private void CommandLogError()
        {
            foreach (var entry in Logging.Log)
            {
                OnStatus(string.Concat(entry, Environment.NewLine));
            }
        }

        /// <summary>
        ///     Commands the log information.
        /// </summary>
        private void CommandLogInfo()
        {
            var message = string.Concat(IrtConst.MessageLogStatistics, Environment.NewLine, IrtConst.MessageErrorCount,
                Logging.Log.Count, Environment.NewLine, IrtConst.MessageLogCount, _log.Count);

            OnStatus(message);
        }

        /// <summary>
        ///     Commands the log full.
        /// </summary>
        private void CommandLogFull()
        {
            foreach (var entry in new List<string>(_log.Values))
            {
                _sendLogs.Invoke(this, entry);
            }
        }

        /// <summary>
        ///     Processes the container.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="parameterPart">Parameter Part.</param>
        private void CommandContainer(string inputString, string parameterPart)
        {
            char[] openParenthesis = { IrtConst.BaseOpen, IrtConst.AdvancedOpen };
            char[] closeParenthesis = { IrtConst.BaseClose, IrtConst.AdvancedClose };

            var check = Irt.CheckMultiple(inputString, openParenthesis, closeParenthesis);

            //Remove outer and first Parenthesis
            parameterPart = Irt.RemoveLastOccurrence(parameterPart, IrtConst.AdvancedClose);
            parameterPart = Irt.RemoveFirstOccurrence(parameterPart, IrtConst.AdvancedOpen);

            if (!check)
            {
                {
                    var log = Logging.SetLastError(IrtConst.ParenthesisError, 0);
                    SetError(log);
                }
            }

            GenerateCommands(parameterPart);
        }

        /// <summary>
        ///     Batch execute of Commands in a file.
        /// </summary>
        /// <param name="parameterPart">The Parameter.</param>
        private void CommandBatchExecute(string parameterPart)
        {
            //Remove Parenthesis
            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseClose, IrtConst.BaseOpen);
            parameterPart = IrtHelper.ReadBatchFile(parameterPart);

            //check if Command Dictionary was empty
            if (parameterPart?.Length == 0)
            {
                SetError(IrtConst.ErrorFileNotFound);
                return;
            }

            GenerateCommands(parameterPart);
        }

        /// <summary>
        ///     Generates the commands.
        /// </summary>
        /// <param name="parameterPart">The parameter part.</param>
        private void GenerateCommands(string parameterPart)
        {
            foreach (var com in Irt.SplitParameter(parameterPart, IrtConst.NewCommand))
            {
                //just because we run a container or a batch, we still have to log it
                _prompt.AddToLog(com);
                HandleInput(com);
            }
        }

        /// <summary>
        ///     Decide the appropriate Action by command
        /// </summary>
        /// <param name="key">Id of command</param>
        /// <param name="parameter">List of Parameters</param>
        /// <returns>Result of our Command</returns>
        private void SetResult(int key, List<string> parameter)
        {
            var com = new OutCommand { Command = key, Parameter = parameter, UsedNameSpace = _nameSpace };

            OnCommand(com);
        }

        /// <summary>
        ///     Set the error Status of the Output command
        /// </summary>
        private void SetError(string error)
        {
            var com = new OutCommand { Command = IrtConst.ErrorParam, Parameter = null, UsedNameSpace = _nameSpace, ErrorMessage = error };
            OnCommand(com);
        }

        /// <summary>
        ///     Sends everything
        /// </summary>
        /// <param name="sendLog">Debug and Status Messages</param>
        private void OnStatus(string sendLog)
        {
            SendInternalLog?.Invoke(this, sendLog);
        }

        /// <summary>
        ///     Only sends Commands
        /// </summary>
        /// <param name="outCommand">Selected User Command</param>
        private void OnCommand(OutCommand outCommand)
        {
            sendCommand?.Invoke(this, outCommand);
        }
    }
}
