/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtInternal.cs
 * PURPOSE:     Handle the Input for all Internal commands. Must be mire clean since it will handle all batch and container Commands.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Interpreter
{
    /// <summary>
    ///     Instance to handle all internal Commands
    /// </summary>
    internal sealed class IrtInternal
    {
        /// <summary>
        ///     The name space
        /// </summary>
        private static string _nameSpace;

        /// <summary>
        ///     The prompt
        /// </summary>
        private static Prompt _prompt;

        /// <summary>
        ///     The commands
        /// </summary>
        private readonly Dictionary<int, InCommand> _commands;

        /// <summary>
        ///     The irt prompt
        /// </summary>
        private readonly IrtPrompt _irtPrompt;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IrtInternal" /> class.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <param name="irtPrompt">The irt prompt.</param>
        /// <param name="nameSpace">The name space.</param>
        public IrtInternal(Dictionary<int, InCommand> commands, IrtPrompt irtPrompt, string nameSpace)
        {
            _commands = commands;
            _irtPrompt = irtPrompt;
            _nameSpace = nameSpace;
        }

		/// <summary>
		///     Handles the internal commands.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="parameter">The parameter.</param>
		/// <param name="prompt">The prompt.</param>
		internal void HandleInternalCommands(int command, List<string> parameter, Prompt prompt)
        {
            _prompt = prompt;

            HandleInternalCommands(command, parameter);
        }

        /// <summary>
        ///     For Internal Commands
        /// </summary>
        /// <param name="command">Key of the command</param>
        /// <param name="parameter"></param>
        private void HandleInternalCommands(int command, IReadOnlyList<string> parameter)
        {
            switch (command)
            {
                case 0:
                    CommandHelp(parameter[0]);
                    break;

                case 1:
                    CommandList();
                    break;

                case 2:
                    CommandUsing(_nameSpace);
                    break;

                case 3:
                    CommandUse(parameter[0]);
                    break;

                case 5:
                    CommandLogError();
                    break;

                case 4:
                    CommandLogInfo();
                    break;

                case 6:
                    CommandLogFull();
                    break;

                case 7:
                    CommandContainer(parameter[0]);
                    break;

                case 8:
                    CommandBatchExecute(parameter[0]);
                    break;

                default:
                    OnStatus(string.Concat(IrtConst.KeyWordNotFoundError, command));
                    break;
            }
        }

        /// <summary>
        ///     Return help for specific command
        /// </summary>
        /// <param name="parameterPart">Parameter about what we want help about</param>
        private void CommandHelp(string parameterPart)
        {
            if (parameterPart == IrtConst.InternalEmptyParameter)
            {
                OnStatus(IrtConst.HelpGeneric);
                return;
            }

            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseOpen, IrtConst.BaseClose);
            var key = Irt.CheckForKeyWord(parameterPart, _commands);

            if (key == IrtConst.ErrorParam)
            {
                SetError(Logging.SetLastError($"{IrtConst.KeyWordNotFoundError}{parameterPart}", 0));
                return;
            }

            var commandInfo = _commands[key];
            OnStatus(
                $"{commandInfo.Command}{IrtConst.FormatDescription}{commandInfo.Description}{IrtConst.FormatCount}{commandInfo.ParameterCount}");
        }

        /// <summary>Command to switch between using.</summary>
        /// <param name="parameterPart">The parameter part.</param>
        private void CommandUse(string parameterPart)
        {
            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseOpen, IrtConst.BaseClose);

            if (!_prompt.CollectedSpaces.ContainsKey(parameterPart))
            {
                OnStatus(IrtConst.ErrorUserSpaceNotFound);
                return;
            }

            _prompt.SwitchNameSpaces(parameterPart);
            OnStatus($"{IrtConst.InformationNamespaceSwitch}{parameterPart}");
        }

        /// <summary>
        ///     List all available Commands
        /// </summary>
        private void CommandList()
        {
            foreach (var com in _commands.Values)
                OnStatus($"{com.Command}{Environment.NewLine}{com.Description}");
        }

        /// <summary>Display all using and the Current in Use.</summary>
        /// <param name="nameSpace">The name space.</param>
        private void CommandUsing(string nameSpace)
        {
            OnStatus($"{IrtConst.Active}{nameSpace}");
            foreach (var key in _prompt.CollectedSpaces.Keys)
                OnStatus(key);
        }

        /// <summary>
        ///     Commands the log.
        /// </summary>
        private void CommandLogError()
        {
            foreach (var entry in Logging.Log)
                OnStatus($"{entry}{Environment.NewLine}");
        }

        /// <summary>
        ///     Commands the log information.
        /// </summary>
        private void CommandLogInfo()
        {
            var message =
                $"{IrtConst.MessageLogStatistics}{Environment.NewLine}{IrtConst.MessageErrorCount}{Logging.Log.Count}{Environment.NewLine}{IrtConst.MessageLogCount}{_prompt.Log.Count}";
            OnStatus(message);
        }

        /// <summary>
        ///     Commands the log full.
        /// </summary>
        private void CommandLogFull()
        {
            foreach (var entry in new List<string>(_prompt.Log.Values))
                _prompt.SendLogs?.Invoke(nameof(IrtInternal), entry);
        }

        /// <summary>
        ///     Processes the container.
        /// </summary>
        /// <param name="parameterPart">Parameter Part.</param>
        private void CommandContainer(string parameterPart)
        {
            parameterPart = Irt.RemoveLastOccurrence(parameterPart, IrtConst.AdvancedClose);
            parameterPart = Irt.RemoveFirstOccurrence(parameterPart, IrtConst.AdvancedOpen);

            GenerateCommands(parameterPart);
        }

        /// <summary>
        ///     Batch execute of Commands in a file.
        /// </summary>
        /// <param name="parameterPart">The Parameter.</param>
        private void CommandBatchExecute(string parameterPart)
        {
            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseOpen, IrtConst.BaseClose);
            parameterPart = IrtHelper.ReadBatchFile(parameterPart);

            if (string.IsNullOrEmpty(parameterPart))
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
                _irtPrompt.HandleInput(com);
            }
        }

        /// <summary>
        ///     Sets the error.
        /// </summary>
        /// <param name="error">The error.</param>
        private void SetError(string error)
        {
            var com = new OutCommand
                { Command = IrtConst.ErrorParam, Parameter = null, UsedNameSpace = _nameSpace, ErrorMessage = error };

            OnCommand(com);
        }

        /// <summary>
        ///     Only sends Commands
        /// </summary>
        /// <param name="outCommand">Selected User Command</param>
        private void OnCommand(OutCommand outCommand)
        {
            _prompt.SendCommands?.Invoke(this, outCommand);
        }

        /// <summary>
        ///     Sends everything
        /// </summary>
        /// <param name="sendLog">Debug and Status Messages</param>
        private void OnStatus(string sendLog)
        {
            _prompt.SendLogs?.Invoke(this, sendLog);
        }
    }
}