/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtFeedback.cs
 * PURPOSE:     Handle some stuff from the User Feedback
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;

namespace Interpreter
{
	/// <summary>
	/// handle the request of the User
	/// </summary>
	internal class IrtHandleFeedback
	{
		/// <summary>
		/// The user feedback
		/// </summary>
		private Dictionary<int, UserFeedback> _userFeedback;

		/// <summary>
		/// The prompt
		/// </summary>
		private Prompt _prompt;

		public IrtHandleFeedback(Dictionary<int, UserFeedback> userFeedback, Prompt prompt)
		{
			_userFeedback = userFeedback;
			_prompt = prompt;
		}

		internal void HandleUserInput(string input)
		{
			// Check if awaited input exists in feedback dictionaries
			if (!(_userFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput) ||
				  IrtConst.InternalFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput)))
			{
				_prompt.CommandRegister.AwaitInput = false;
				return;
			}

			// Get the feedback from either dictionary
			var feedback = _userFeedback.ContainsKey(_prompt.CommandRegister.AwaitedInput)
				? _userFeedback[_prompt.CommandRegister.AwaitedInput]
				: IrtConst.InternalFeedback[_prompt.CommandRegister.AwaitedInput];

			// Show initial message if not already shown
			if (!_prompt.CommandRegister.InitialMessageShown) _prompt.SendLogs?.Invoke(this, feedback.ToString());

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
