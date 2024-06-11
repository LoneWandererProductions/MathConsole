/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IPrompt.cs
 * PURPOSE:     The Prompt Interface
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global, not used yet
// ReSharper disable UnusedMemberInSuper.Global

using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    ///     The IPrompt interface.
    /// </summary>
    internal interface IPrompt
    {
        /// <summary>
        ///     Starts the Sender and Interpreter.
        /// </summary>
        /// <param name="commands">The command register.</param>
        /// <param name="userSpace">The user space of the register.</param>
        /// <param name="extension">Optional extension methods.</param>
        void Initiate(Dictionary<int, InCommand> commands, string userSpace,
            Dictionary<int, InCommand> extension = null);

        /// <summary>
        ///     Adds further command namespaces.
        /// </summary>
        /// <param name="commands">The command register.</param>
        /// <param name="userSpace">The user space of the register.</param>
        /// <param name="extension">Optional extension methods.</param>
        void AddCommands(Dictionary<int, InCommand> commands, string userSpace,
            Dictionary<int, InCommand> extension = null);

        /// <summary>
        ///     Starts the window, if we want to use the included window.
        /// </summary>
        void StartWindow();

        /// <summary>
        ///     Sends method messages to the window.
        /// </summary>
        /// <param name="message">The message to send to the window.</param>
        void CallbacksWindow(string message);

        /// <summary>
        ///     Sends a callback message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Callback(string message);

        /// <summary>
        ///     Starts the console.
        /// </summary>
        /// <param name="input">The console input.</param>
        void StartConsole(string input);
    }
}