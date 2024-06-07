/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtExtension.cs
 * PURPOSE:     Handle the Extension Methods.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Interpreter
{
    /// <summary>
    /// Handle the Extensions
    /// </summary>
    internal static class IrtExtension
    {
        /// <summary>
        ///     Checks for extension.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="extensionCommands">The extension commands.</param>
        /// <returns>Status and Extension Commands</returns>
        internal static (ExtensionCommands Extension, int Status) CheckForExtension(string input, string nameSpace,
            Dictionary<int, InCommand> extensionCommands)
        {
            var exCommand = new ExtensionCommands();

            // Find periods that are not inside {} or ()
            const string pattern = @"\.(?![^{}]*\})(?![^(]*\))";
            var regex = new Regex(pattern);

            // Split the input based on the regex pattern
            var result = regex.Split(input);

            // Determine the split result and handle accordingly
            return result.Length switch
            {
                1 => (null, IrtConst.NoSplitOccurred), // No split occurred
                > 2 => (null, IrtConst.Error), // More than one extension found, more are not planned yet
                _ => ProcessExtension(result[1], nameSpace, extensionCommands, exCommand) // Process the extension
            };
        }

        /// <summary>
        ///     Processes the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="extensionCommands">The extension commands.</param>
        /// <param name="exCommand">The ex command.</param>
        /// <returns>Status and Extension Commands</returns>
        private static (ExtensionCommands Extension, int Status) ProcessExtension(string extension, string nameSpace,
            Dictionary<int, InCommand> extensionCommands, ExtensionCommands exCommand)
        {
            var param = Irt.CheckInternalCommands(extension, IrtConst.InternalExtensionCommands);

            if (!string.IsNullOrEmpty(param))
            {
                var parameters = ExtractParameters(param, extension);
                exCommand.ExtensionNameSpace = IrtConst.InternalNameSpace;
                exCommand.ExtensionParameter = parameters;
                exCommand.InternalCommand = param;

                // TODO: Check if Parentheses are correct and Parameter Count
                return (exCommand, IrtConst.InternalExtensionFound);
            }

            var key = Irt.CheckForKeyWord(extension, extensionCommands);
            if (key == IrtConst.ErrorParam)
            {
                return (null, IrtConst.Error);
            }

            // Validate parameter count and parentheses
            if (!ValidateParameters(extension, key, extensionCommands))
            {
                return (null, IrtConst.Error);
            }

            var command = extensionCommands[key].Command.ToUpper(CultureInfo.InvariantCulture);
            var commandParameters = ExtractParameters(command, extension);
            exCommand.ExtensionNameSpace = nameSpace;
            exCommand.ExtensionCommand = key;
            exCommand.ExtensionParameter = commandParameters;

            // TODO: Check if Parentheses are correct and Parameter Count
            return (exCommand, IrtConst.NamespaceExtensionFound);
        }

        /// <summary>
        ///     Extracts the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>cleaned Parameter</returns>
        private static List<string> ExtractParameters(string command, string extension)
        {
            var parameterPart = Irt.RemoveWord(command, extension);
            return Irt.SplitParameter(parameterPart, IrtConst.Splitter);
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="com">The COM.</param>
        /// <returns>If Parameters are correct</returns>
        private static bool ValidateParameters(string input, int key, Dictionary<int, InCommand> com)
        {
            return com[key].ParameterCount == 0 || Irt.SingleCheck(input);
        }
    }
}