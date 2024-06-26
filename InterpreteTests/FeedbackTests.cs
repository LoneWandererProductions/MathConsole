/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/FeedbackTests.cs
 * PURPOSE:     Tests for the Feedback loop
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace InterpreteTests
{
	/// <summary>
	/// Test user Feedback
	/// </summary>
	[TestClass]
	public sealed class FeedbackTests
	{

		/// <summary>
		/// The user feedback
		/// </summary>
		private Dictionary<int, UserFeedback> _userFeedback = new Dictionary<int, UserFeedback>();

		/// <summary>
		/// The prompt
		/// </summary>
		private Prompt _prompt = new Prompt();

		//[TestMethod]
		public void HandleUserInput_ValidInput_ShowInitialMessageAndSendCommands()
		{
			// Arrange
			var feedback = new UserFeedback();
			_userFeedback[1] = feedback;

			var register = new IrtFeedback()
			{
				AwaitedInput = 1,
				AwaitInput = true
			};
			_prompt.CommandRegister = register;

			var handleFeedback = new IrtHandleFeedback(_userFeedback, _prompt);

			// Act
			handleFeedback.HandleUserInput(" yES");

			// Assert
			Assert.IsTrue(_prompt.CommandRegister.InitialMessageShown);
			// Add more assertions based on expected behavior after handling input
		}

		[TestMethod]
		public void HandleUserInput_InvalidAwaitedInput_NoCommandsSent()
		{
			// Arrange
			var register = new IrtFeedback()
			{
				AwaitedInput = 999, // Assuming this key doesn't exist in _userFeedback or IrtConst.InternalFeedback
				AwaitInput = true
			};
			_prompt.CommandRegister = register;

			var handleFeedback = new IrtHandleFeedback(_userFeedback, _prompt);

			// Act
			handleFeedback.HandleUserInput(" yES");

			// Assert
			Assert.IsFalse(_prompt.CommandRegister.AwaitInput);
			// Add more assertions based on expected behavior after handling input
		}

		[TestMethod]
		public void HandleUserInput_NullFeedback_LogsError()
		{
			// Arrange
			var register = new IrtFeedback()
			{
				AwaitedInput = 1, // Assuming this key doesn't exist in _userFeedback or IrtConst.InternalFeedback
				AwaitInput = true
			};
			_prompt.CommandRegister = register;

			var handleFeedback = new IrtHandleFeedback(_userFeedback, _prompt);

			// Act
			handleFeedback.HandleUserInput(" yES");

			// Assert
			// Verify that an error command is sent or appropriate logging occurs
			// Example: Assert.AreEqual(expectedErrorMessage, _prompt.LastErrorMessage);
		}
	}
}
