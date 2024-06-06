/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     OutCommand
 * FILE:        Interpreter/ExtensionCommands.cs
 * PURPOSE:     Handles potential Extension Methods from Internal and external
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    ///     Extensions for the existing commands
    /// </summary>
    public sealed class ExtensionCommands
    {
        /// <summary>
        ///     Gets the used name space.
        /// </summary>
        /// <value>
        ///     The used name space.
        /// </value>
        /// <summary>
        ///     Gets the error message.
        /// </summary>
        /// <value>
        ///     The error message.
        /// </value>
        public string ExtensionError { get; internal init; }

        /// <summary>
        ///     Gets the used name space.
        /// </summary>
        /// <value>
        ///     The used name space.
        /// </value>
        public string ExtensionNameSpace { get; internal set; }

        /// <summary>
        ///     Gets or sets the command.
        /// </summary>
        public int ExtensionCommand { get; internal set; }

        /// <summary>
        ///     Gets or sets the parameter.
        /// </summary>
        public List<string> ExtensionParameter { get; internal set; }

        /// <summary>
        ///     Gets or sets the internal command.
        /// </summary>
        /// <value>
        ///     The internal command.
        /// </value>
        public string InternalCommand { get; set; }
    }
}