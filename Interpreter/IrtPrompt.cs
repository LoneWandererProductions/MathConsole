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
        ///     Extension Command Register
        /// </summary>
        private static Dictionary<int, InCommand> _extension;

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
            //set the basic parameter
            _com = use.Commands;
            _extension = use.ExtensionCommands;
            _nameSpace = use.UserSpaceName;
            _irtInternal = new IrtInternal(use.Commands, this, use.UserSpaceName, _prompt);
            //prepare our Extension handler
            _irtExtension = new IrtExtension();
            //notify log about loading up
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
                OnStatus(IrtConst.HelpGeneric);
                return;
            }

            //first Check if the Parenthesis are right?
            if (!Irt.ValidateParameters(inputString))
            {
                SetErrorWithLog(IrtConst.ParenthesisError);
                return;
            }

            // Check for extensions in the internal namespace first, then in the external namespace if needed
            var extensionResult = _irtExtension.CheckForExtension(_inputString, IrtConst.InternalNameSpace,
                IrtConst.InternalExtensionCommands);

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
                        ProcessExtensionInternal(extensionResult.Extension);
                    else
                        ProcessInput(inputString, extensionResult.Extension);

                    return;
            }

            ProcessInput(inputString);
        }


        /// <summary>
        ///     Processes the internal extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        private void ProcessExtensionInternal(ExtensionCommands extension)
        {
            switch (extension.ExtensionCommand)
            {
                // switch internal Namespace
                case 0:
                    //first switch Namespace
                    _prompt.SwitchNameSpaces(extension.ExtensionParameter[0]);
                    //set input without Extension Method and execute cleaned command
                    _prompt.ConsoleInput(extension.BaseCommand);
                    break;
                case 1:
                    break;
            }
        }

        /// <summary>
        ///     Processes the input string.
        /// </summary>
        /// <param name="inputString">Input string</param>
        /// <param name="extension">All Extensions</param>
        private void ProcessInput(string inputString, ExtensionCommands extension = null)
        {
            var key = Irt.CheckForKeyWord(inputString, IrtConst.InternCommands);
            if (key != IrtConst.Error)
            {
                _irtInternal.ProcessInput(key, inputString);
                return;
            }

            if (_com == null)
            {
                SetError(IrtConst.ErrorNoCommandsProvided);
                return;
            }

            key = Irt.CheckForKeyWord(inputString, _com);

            if (key == IrtConst.Error)
            {
                SetErrorWithLog(IrtConst.KeyWordNotFoundError, _inputString);
                return;
            }

            //do our normal Checks for User Commands
            var (status, splitParameter) = Irt.ProcessParameters(inputString, key, _com);

            //actual not possible yet, this must be implemented from user side and I have not build support for it yet
            var parameter = status == IrtConst.ParameterCommand
                ? Irt.SplitParameter(splitParameter, IrtConst.Splitter)
                : new List<string> {splitParameter};

            //check for Parameter Overload
            var check = Irt.CheckOverload(_com[key].Command, parameter.Count, _com);

            if (check == null)
            {
                SetErrorWithLog(IrtConst.SyntaxError);
                return;
            }

            //add optional Extension data
            SetResult((int)check, parameter, extension);
        }

        /// <summary>
        ///     Cleans the input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Mostly cleaned Input string and all Uppercase.</returns>
        private static bool CleanInputString(ref string input)
        {
            // Ensure the input string has well-formed parentheses and convert it to uppercase once.
            input = Irt.WellFormedParenthesis(input).ToUpper(CultureInfo.InvariantCulture);

            // Define the sets of open and close parentheses to check for.
            var openParenthesis = new[] { IrtConst.BaseOpen, IrtConst.AdvancedOpen };
            var closeParenthesis = new[] { IrtConst.BaseClose, IrtConst.AdvancedClose };

            // Verify that all types of parentheses are correctly paired and balanced in the input string.
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
        ///     Decide the appropriate Action by command
        ///     Optional set the User Input
        /// </summary>
        /// <param name="key">Id of command</param>
        /// <param name="parameter">List of Parameters</param>
        /// <param name="extensionCommands"></param>
        /// <returns>Result of our Command</returns>
        private void SetResult(int key, List<string> parameter, ExtensionCommands extensionCommands)
        {
            var com = new OutCommand
            {
                Command = key, Parameter = parameter, UsedNameSpace = _nameSpace, ExtensionCommand = extensionCommands
            };

            //does the Command come with needed User Feedback?
            if(_com[key].FeedbackId == 0)
            {
                _prompt.SendCommand(this, com);
            }
            //if yes inform the prompt to handle it correctly
            else
            {
                _prompt.CommandRegister = new IrtFeedback
                {
                    AwaitedOutput = com, AwaitInput = true, AwaitedInput = _com[key].FeedbackId
                };
            }
        }

        /// <summary>
        ///     Set the error Status of the Output command
        /// </summary>
        private void SetError(string error)
        {
            var com = new OutCommand
                { Command = IrtConst.Error, Parameter = null, UsedNameSpace = _nameSpace, ErrorMessage = error };


            _prompt.SendCommand(this, com);
        }

        /// <summary>
        ///     Sends everything
        /// </summary>
        /// <param name="sendLog">Debug and Status Messages</param>
        private void OnStatus(string sendLog)
        {
            SendInternalLog?.Invoke(this, sendLog);
        }
    }
}