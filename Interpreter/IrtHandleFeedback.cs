/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtFeedback.cs
 * PURPOSE:     Handle some stuff from the User Feedback
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// handle the request of the User
    /// </summary>
    internal sealed class IrtHandleFeedback
    {
        /// <summary>
        /// The user feedback
        /// </summary>
        private readonly Dictionary<int, UserFeedback> _userFeedback;

        /// <summary>
        /// The prompt
        /// </summary>
        private readonly Prompt _prompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="IrtHandleFeedback"/> class.
        /// </summary>
        /// <param name="userFeedback">The user feedback.</param>
        /// <param name="prompt">The prompt.</param>
        public IrtHandleFeedback(Dictionary<int, UserFeedback> userFeedback, Prompt prompt)
        {
            _userFeedback = userFeedback;
            _prompt = prompt;
        }

        /// <summary>
        /// Handles the user input.
        /// </summary>
        /// <param name="input">The input.</param>
        internal void HandleUserInput(string input)
        {
            //trim whitespaces and us Uppercase
            input = input.Trim().ToUpper();

            // Check if awaited input exists in feedback dictionaries
            if (!(_userFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput) ||
                  IrtConst.InternalFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput)))
            {
                _prompt.CommandRegister.AwaitInput = false;
                return;
            }

            // Get the feedback from either user defined dictionary or from internal, if the key is negative
            UserFeedback feedback = null;
            if (_userFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput))
            {
                feedback = _userFeedback[_prompt.CommandRegister.AwaitedInput];
            }
            else if (IrtConst.InternalFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput))
            {
                feedback = IrtConst.InternalFeedback[_prompt.CommandRegister.AwaitedInput];
            }

            //check if we have some generic information about the options, if not, well return and show some Errors
            if (feedback == null || feedback.Options == null)
            {
                var error = Logging.SetLastError("No Options are available.", 0);
                var com = new OutCommand
                    { Command = IrtConst.Error, Parameter = null, ErrorMessage = error };

                _prompt.SendCommand(this, com);
                return;
            }

            // Show initial message if not already shown
            if (!_prompt.CommandRegister.InitialMessageShown)
            {
                _prompt.CommandRegister.InitialMessageShown = true;
                _prompt.SendLogs?.Invoke(this, feedback.ToString());
            }

            // Send awaited output command if not awaiting input
            if (!_prompt.CommandRegister.AwaitInput) _prompt.SendCommands(this, _prompt.CommandRegister.AwaitedOutput);

            // Process the user input
            switch (input.ToUpper())
            {
                case var command when command == nameof(AvailableFeedback.Yes).ToUpper():
                    HandleOption(feedback, AvailableFeedback.Yes, nameof(AvailableFeedback.Yes));
                    break;
                case var command when command == nameof(AvailableFeedback.No).ToUpper():
                    HandleOption(feedback, AvailableFeedback.No, nameof(AvailableFeedback.No), true);
                    break;
                case var command when command == nameof(AvailableFeedback.Cancel).ToUpper():
                    HandleOption(feedback, AvailableFeedback.Cancel, nameof(AvailableFeedback.Cancel), true);
                    break;
                default:
                    _prompt.SendLogs?.Invoke(this, "Option not allowed.");

                    break;
            }
        }

        /// <summary>
        ///     Handles the option.
        /// </summary>
        /// <param name="feedback">The feedback.</param>
        /// <param name="option">The option.</param>
        /// <param name="optionName">Name of the option.</param>
        /// <param name="terminate">if set to <c>true</c> [terminate].</param>
        private void HandleOption(UserFeedback feedback, AvailableFeedback option, string optionName,
            bool terminate = false)
        {
            if (!feedback.Options.ContainsKey(option))
            {
                _prompt.SendLogs(this, "Option not allowed.");
                return;
            }

            _prompt.SendLogs(this, $"You selected {optionName}");
            if (terminate)
            {
                _prompt.CommandRegister.AwaitInput = false;
                _prompt.CommandRegister = null;
            }
            else if (_prompt.CommandRegister.AwaitInput)
            {
                _prompt.SendCommands(this, _prompt.CommandRegister.AwaitedOutput);
            }
        }
    }
}
