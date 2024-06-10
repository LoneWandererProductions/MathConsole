/*COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter / IrtPrompt.cs
 * PURPOSE:     Handle the Input
 * PROGRAMER:   Peter Geinitz(Wayfarer)
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
        ///     The prompt
        /// </summary>
        private readonly Prompt _prompt;

        /// <summary>
        ///     The original input string
        /// </summary>
        private string _inputString;

        /// <summary>
        ///     The irt extension
        /// </summary>
        private IrtExtension _irtExtension;

        /// <summary>
        ///     The irt internal
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
        ///     Send selected Command to the Subscriber
        /// </summary>
        internal event EventHandler<OutCommand> SendCommand;

        /// <summary>
        ///     Send selected Command to the Subscriber
        /// </summary>
        internal event EventHandler<string> SendInternalLog;

        /// <summary>
        ///     Get the Engine Running
        /// </summary>
        /// <param name="use">The Command Structure</param>
        internal void Initiate(UserSpace use)
        {
            _com = use.Commands;
            _extension = use.ExtensionCommands;
            _nameSpace = use.UserSpaceName;
            _irtInternal = new IrtInternal(use.Commands, this, use.UserSpaceName);
            _irtExtension = new IrtExtension();
            var log = Logging.SetLastError(IrtConst.InformationStartup, 2);
            OnStatus(log);
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
        ///     will do all the work
        /// </summary>
        /// <param name="inputString">Input string</param>
        /// <returns>Results of our commands</returns>
        internal void HandleInput(string inputString)
        {
            _inputString = inputString;

            var isValid = CleanInputString(ref inputString);

            if (!isValid)
            {
                SetError(Logging.SetLastError(IrtConst.ParenthesisError, 0));
                return;
            }

            if (IsCommentCommand(inputString))
            {
                Trace.WriteLine(inputString);
                return;
            }

            if (IsHelpCommand(inputString))
            {
                OnStatus(IrtConst.HelpGeneric);
                return;
            }


            var extensionResult = _irtExtension.CheckForExtension(_inputString, IrtConst.InternalNameSpace,
                IrtConst.InternalExtensionCommands);
            if (extensionResult.Status == IrtConst.Error)
                extensionResult = _irtExtension.CheckForExtension(_inputString, _nameSpace, _extension);

            switch (extensionResult.Status)
            {
                case IrtConst.Error:
                    //TODO handle
                    break;
                case IrtConst.ParameterMismatch:
                    //TODO handle
                    break;
                case IrtConst.ExtensionFound:
                    if (extensionResult.Extension.ExtensionNameSpace == IrtConst.InternalNameSpace)
                        ProcessInputInternal(inputString, extensionResult);
                    else ProcessInput(inputString, extensionResult);

                    return;
            }

            ProcessInput(inputString);
        }

        private void ProcessInputInternal(string inputString, (ExtensionCommands Extension, int Status) extensionResult)
        {
            throw new NotImplementedException();
        }

        private void ProcessInput(string inputString, (ExtensionCommands Extension, int Status) extensionResult)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Processes the input string.
        /// </summary>
        /// <param name="inputString">Input string</param>
        private void ProcessInput(string inputString)
        {
            var key = Irt.CheckForKeyWord(inputString, IrtConst.InternCommands);

            (int Status, string Parameter) parameterPart;

            List<string> parameter;

            //checks if it was an internal Command.
            if (key != IrtConst.ErrorParam)
            {
                if (!ValidateParameters(inputString, key, IrtConst.InternCommands)) return;

                parameterPart = ProcessParameters(inputString, key, IrtConst.InternCommands);

                parameter = (parameterPart.Status == 1) ? Irt.SplitParameter(parameterPart.Parameter, IrtConst.Splitter) : new List<string> { parameterPart.Parameter };

                _irtInternal.HandleInternalCommands(key, parameter, _prompt);
                return;
            }

            if (_com == null)
            {
                SetError(IrtConst.ErrorNoCommandsProvided);
                return;
            }

            key = Irt.CheckForKeyWord(inputString, _com);

            if (key == IrtConst.ErrorParam)
            {
                SetErrorWithLog(IrtConst.KeyWordNotFoundError, _inputString);
                return;
            }

            if (!ValidateParameters(inputString, key, _com)) return;

            parameterPart = ProcessParameters(inputString, key, _com);

            parameter = parameterPart.Status == 1
                ? Irt.SplitParameter(parameterPart.Parameter, IrtConst.Splitter)
                : new List<string> { parameterPart.Parameter };

            //check for Parameter Overload
            var check = Irt.CheckOverload(_com[key].Command, parameter.Count, _com);

            if (check == null)
            {
                SetErrorWithLog(IrtConst.SyntaxError);
                return;
            }

            SetResult((int)check, parameter);
        }

        /// <summary>
        ///     Cleans the input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Mostly cleaned Input string and all Uppercase.</returns>
        private static bool CleanInputString(ref string input)
        {
            input = Irt.WellFormedParenthesis(input).ToUpper(CultureInfo.CurrentCulture)
                .ToUpper(CultureInfo.InvariantCulture);

            var openParenthesis = new[] { IrtConst.BaseOpen, IrtConst.AdvancedOpen };
            var closeParenthesis = new[] { IrtConst.BaseClose, IrtConst.AdvancedClose };

            //check if all the Parenthesis are actual in place
            return Irt.CheckMultiple(input, openParenthesis, closeParenthesis);
        }

        /// <summary>
        ///     Determines whether [is comment command] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///     <c>true</c> if [is comment command] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCommentCommand(string input)
        {
            return input.StartsWith(IrtConst.CommentCommand, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Determines whether [is help command] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///     <c>true</c> if [is help command] [the specified input]; otherwise, <c>false</c>.
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
        /// <param name="additionalInfo">The additional information.</param>
        private void SetErrorWithLog(string errorMessage, string additionalInfo = "")
        {
            var log = Logging.SetLastError(errorMessage, 0);
            SetError(string.Concat(log, additionalInfo));
        }

        /// <summary>
        ///     Validates the parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="commands">The current Commands</param>
        /// <returns>Check if Parameter is valid</returns>
        private bool ValidateParameters(string input, int key, IReadOnlyDictionary<int, InCommand> commands)
        {
            if (commands[key].ParameterCount == 0 || Irt.SingleCheck(input)) return true;

            SetErrorWithLog(IrtConst.ParenthesisError);
            return false;
        }

        /// <summary>
        ///     Processes the parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="commands">The commands in use.</param>
        /// <returns>return Parameter</returns>
        private static (int Status, string Parameter) ProcessParameters(string input, int key,
            IReadOnlyDictionary<int, InCommand> commands)
        {
            var command = commands[key].Command.ToUpper(CultureInfo.InvariantCulture);
            var parameterPart = Irt.RemoveWord(command, input);

            return parameterPart.StartsWith(IrtConst.AdvancedOpen)
                ? (0, parameterPart)
                : (1, Irt.RemoveParenthesis(parameterPart, IrtConst.BaseClose, IrtConst.BaseOpen));
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
            var com = new OutCommand
            { Command = IrtConst.ErrorParam, Parameter = null, UsedNameSpace = _nameSpace, ErrorMessage = error };

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
            SendCommand?.Invoke(this, outCommand);
        }
    }
}