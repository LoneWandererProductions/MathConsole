﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtFeedback.cs
 * PURPOSE:     Handle some stuff from the User Feedback
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace Interpreter
{
    /// <summary>
    ///     Control Object that checks if User Feedback is needed
    /// </summary>
    internal sealed class IrtFeedback
    {
        /// <summary>
        ///     Gets or sets a value indicating whether [initial message was shown].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [initial message shown]; otherwise, <c>false</c>.
        /// </value>
        internal bool InitialMessageShown { get; set; }

        /// <summary>
        ///     The await input check, if true await correct answer
        /// </summary>
        /// <value>
        ///     <c>true</c> if [await input]; otherwise, <c>false</c>.
        /// </value>
        internal bool AwaitInput { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [last selected option].
        ///     Will be used for if Checks.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [last selected option]; otherwise, <c>false</c>.
        /// </value>
        internal bool? LastSelectedOption { get; set; }

        /// <summary>
        ///     Gets or sets the awaited input Id,
        ///     Can be user Dictionary or the Internal Feedback Id. This just offers the options we are allowed to select.
        /// </summary>
        /// <value>
        ///     The awaited input Id.
        /// </value>
        internal int AwaitedInput { get; init; }

        /// <summary>
        ///     Gets or sets the awaited output.
        /// </summary>
        /// <value>
        ///     The awaited output.
        /// </value>
        internal OutCommand AwaitedOutput { get; set; }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        /// <param name="lastSelectedOption">if set to <c>true</c> [last selected option].</param>
        internal void Clear(bool? lastSelectedOption)
        {
            AwaitInput = false;
            AwaitedOutput = null;
            LastSelectedOption = lastSelectedOption;
        }
    }
}