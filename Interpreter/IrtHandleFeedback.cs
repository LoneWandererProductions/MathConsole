/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtFeedback.cs
 * PURPOSE:     Handles user feedback requests and responses
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    ///     Handles user feedback interactions.
    /// </summary>
    internal sealed class IrtHandleFeedback
    {
        /// <summary>
        ///     The prompt instance.
        /// </summary>
        private readonly Prompt _prompt;

        /// <summary>
        ///     The user feedback dictionary.
        /// </summary>
        private readonly Dictionary<int, UserFeedback> _userFeedback;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IrtHandleFeedback"/> class.
        /// </summary>
        /// <param name="prompt">The prompt instance.</param>
        /// <param name="userFeedback">The user feedback dictionary.</param>
        /// <param name="use">The current user space.</param>
        public IrtHandleFeedback(Prompt prompt, Dictionary<int, UserFeedback> userFeedback, UserSpace use)
        {
            _prompt = prompt;
            _userFeedback = userFeedback ?? new Dictionary<int, UserFeedback>();
            Use = use;
        }

        /// <summary>
        ///     Gets or sets the current user space.
        /// </summary>
        internal UserSpace Use { get; set; }

        /// <summary>
        ///     Handles the user input.
        /// </summary>
        /// <param name="input">The user input.</param>
        internal void HandleUserInput(string input)
        {
            input = input.Trim().ToUpper();

            if (!(_userFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput) ||
                  IrtConst.InternalFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput)))
            {
                _prompt.CommandRegister.AwaitInput = false;
                _prompt.SendLogs(this, IrtConst.ErrorFeedbackOptions);
                return;
            }

            var feedback = _userFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput)
                ? _userFeedback[_prompt.CommandRegister.AwaitedInput]
                : IrtConst.InternalFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput)
                    ? IrtConst.InternalFeedback[_prompt.CommandRegister.AwaitedInput]
                    : null;

            if (feedback?.Options == null)
            {
                var error = Logging.SetLastError(IrtConst.ErrorFeedbackOptions, 0);
                var com = new OutCommand { Command = IrtConst.Error, Parameter = null, ErrorMessage = error };
                _prompt.SendCommand(this, com);
                return;
            }

            if (!_prompt.CommandRegister.InitialMessageShown)
            {
                _prompt.CommandRegister.InitialMessageShown = true;
                _prompt.SendLogs?.Invoke(this, feedback.ToString());
            }

            if (!_prompt.CommandRegister.AwaitInput)
            {
                _prompt.SendCommands(this, _prompt.CommandRegister.AwaitedOutput);
            }

            if (input == nameof(AvailableFeedback.Yes).ToUpper())
            {
                HandleOption(feedback, AvailableFeedback.Yes, nameof(AvailableFeedback.Yes));
            }
            else if (input == nameof(AvailableFeedback.No).ToUpper())
            {
                HandleOption(feedback, AvailableFeedback.No, nameof(AvailableFeedback.No));
            }
            else if (input == nameof(AvailableFeedback.Cancel).ToUpper())
            {
                HandleOption(feedback, AvailableFeedback.Cancel, nameof(AvailableFeedback.Cancel));
            }
            else
            {
                _prompt.SendLogs?.Invoke(this, IrtConst.ErrorFeedbackOptionNotAllowed);
            }
        }

        /// <summary>
        ///     Handles the specified feedback option.
        /// </summary>
        /// <param name="feedback">The user feedback.</param>
        /// <param name="option">The selected option.</param>
        /// <param name="optionName">The name of the selected option.</param>
        private void HandleOption(UserFeedback feedback, AvailableFeedback option, string optionName)
        {
            if (!feedback.Options.ContainsKey(option))
            {
                _prompt.SendLogs(this, IrtConst.ErrorFeedbackOptionNotAllowed);
                return;
            }

            _prompt.SendLogs(this, $"{IrtConst.FeedbackMessage} {optionName}");

            switch (option)
            {
                case AvailableFeedback.Cancel:
                    _prompt.CommandRegister.Clear(null);
                    _prompt.CommandRegister.LastSelectedOption = null;
                    break;
                case AvailableFeedback.No:
                    _prompt.CommandRegister.Clear(false);
                    _prompt.CommandRegister.LastSelectedOption = false;
                    break;
                case AvailableFeedback.Yes:
                    var command = _prompt.CommandRegister.AwaitedOutput;

                    if (_prompt.CommandRegister.IsInternalCommand)
                    {
                        _prompt.CommandRegister.HandleInternalCommand();
                        _prompt.CommandRegister.Clear(true);
                    }
                    else if (command == null)
                    {
                        _prompt.SendLogs(this, IrtConst.ErrorFeedbackOptions);
                        _prompt.CommandRegister.Clear(null);
                    }
                    else
                    {
                        _prompt.SendCommands(this, command);
                        _prompt.CommandRegister.Clear(true);
                    }

                    break;
            }
        }
    }
}
