/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtExtension.cs
 * PURPOSE:     Handle the Extension Methods.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Interpreter
{
    /// <summary>
    ///     Handle the Extensions
    /// </summary>
    internal sealed class IrtExtension
    {
        /// <summary>
        ///     Checks for extension.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="extensionCommands">The extension commands.</param>
        /// <returns>
        ///     Status and Extension Commands
        /// </returns>
        internal (ExtensionCommands Extension, int Status) CheckForExtension(string input, string nameSpace,
            Dictionary<int, InCommand> extensionCommands)
        {
            var exCommand = new ExtensionCommands();

            // Find periods that are not inside {} or ()
            var regex = new Regex(IrtConst.RegexParenthesisOutsidePattern);

            // Split the input based on the regex pattern
            var result = regex.Split(input);

            // Determine the split result and handle accordingly
            switch (result.Length)
            {
                case 1:
                    return (null, IrtConst.NoSplitOccurred);
                case > 2:
                    return (null, IrtConst.Error);
                default:
                    return ProcessExtension(result[0], result[1], nameSpace, extensionCommands, exCommand);
            }
        }

        /// <summary>
        ///     Processes the extension.
        /// </summary>
        /// <param name="baseCommand">The base command</param>
        /// <param name="extension">The extension.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="extensionCommands">The extension commands.</param>
        /// <param name="exCommand">The ex command.</param>
        /// <returns>
        ///     Status and Extension Commands
        /// </returns>
        private static (ExtensionCommands Extension, int Status) ProcessExtension(string baseCommand, string extension,
            string nameSpace,
            Dictionary<int, InCommand> extensionCommands, ExtensionCommands exCommand)
        {
            var key = Irt.CheckForKeyWord(extension, extensionCommands);

            if (key == IrtConst.ErrorParam) return (null, IrtConst.Error);

            // Validate parameter count and parentheses
            if (!ValidateParameters(extension, key, extensionCommands)) return (null, IrtConst.Error);

            var command = extensionCommands[key].Command;
            var commandParameters = ExtractParameters(command, extension);

            //check for Parameter Overload
            var check = Irt.CheckOverload(extensionCommands[key].Command, commandParameters.Count, extensionCommands);
            if (check == null) return (null, IrtConst.ParameterMismatch);

            exCommand.ExtensionNameSpace = nameSpace;
            exCommand.ExtensionCommand = key;
            exCommand.ExtensionParameter = commandParameters;
            exCommand.BaseCommand = baseCommand;

            return (exCommand, IrtConst.ExtensionFound);
        }

        /// <summary>
        ///     Extracts the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>cleaned Parameter</returns>
        private static List<string> ExtractParameters(string command, string extension)
        {
            //get Parameter of Extension
            var parameterPart = Irt.RemoveWord(command, extension);
            //remove Parenthesis
            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseOpen, IrtConst.BaseClose);
            //split if multiple do exist
            return Irt.SplitParameter(parameterPart, IrtConst.Splitter);
        }

        /// <summary>
        ///     Validates the parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="com">The COM.</param>
        /// <returns>If Parameters are correct</returns>
        private static bool ValidateParameters(string input, int key, IReadOnlyDictionary<int, InCommand> com)
        {
            return com[key].ParameterCount == 0 || Irt.SingleCheck(input);
        }
    }
}