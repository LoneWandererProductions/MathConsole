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
        ///     The regex, Find periods that are not inside {} or ()
        /// </summary>
        private static readonly Regex Regex = new(IrtConst.RegexParenthesisOutsidePattern);

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

            // Split the input based on the regex pattern
            var result = Regex.Split(input);

            // Determine the split result and handle accordingly
            switch (result.Length)
            {
                case 1:
                    return (null, IrtConst.NoSplitOccurred);
                case > 2:
                    return (null, IrtConst.Error); // Too many periods outside of parentheses
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
            string nameSpace, Dictionary<int, InCommand> extensionCommands, ExtensionCommands exCommand)
        {
            // Check if the extension command exists in the provided extensionCommands
            var commandKey = Irt.CheckForKeyWord(extension, extensionCommands);
            if (commandKey == IrtConst.ErrorParam) return (null, IrtConst.Error);

            // Extract the command string and parameters
            var command = extensionCommands[commandKey].Command;
            var (status, parameters) = ExtractParameters(command, extension);
            if (status == IrtConst.Error) return (null, IrtConst.Error);

            // Validate the Parenthesis logic
            if (!ValidateParameters(extension)) return (null, IrtConst.ParenthesisMismatch);

            // Validate the parameter count of the extension command
            if (!ValidateParameters(commandKey, parameters.Count, extensionCommands))
            {
                // Check for parameter overload
                var overloadCheck = Irt.CheckOverload(extensionCommands[commandKey].Command, parameters.Count,
                    extensionCommands);
                if (overloadCheck == null) return (null, IrtConst.ParameterMismatch);

                commandKey = (int)overloadCheck;
            }

            // Set the extension command details
            exCommand.ExtensionNameSpace = nameSpace;
            exCommand.ExtensionCommand = commandKey;
            exCommand.ExtensionParameter = parameters;
            exCommand.BaseCommand = baseCommand;

            return (exCommand, IrtConst.ExtensionFound);
        }


        /// <summary>
        ///     Extracts the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>cleaned Parameter</returns>
        private static (int Status, List<string> Parameter) ExtractParameters(string command, string extension)
        {
            // Remove the command part from the extension to get the parameter part
            var parameterPart = Irt.RemoveWord(command, extension);

            // Remove parentheses from the parameter part
            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseOpen, IrtConst.BaseClose);
            if (parameterPart == IrtConst.ParenthesisError) return (IrtConst.Error, null);

            // Split the parameter part into individual parameters if multiple exist
            var parameterList = Irt.SplitParameter(parameterPart, IrtConst.Splitter);

            return (IrtConst.ExtensionFound, parameterList);
        }


        /// <summary>
        ///     Validates the parameters.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parametersCount">Count of parameter</param>
        /// <param name="commands">The commands.</param>
        /// <returns>If Parameters are correct</returns>
        private static bool ValidateParameters(int key, int parametersCount,
            IReadOnlyDictionary<int, InCommand> commands)
        {
            // Check if the command requires parameters or if the input has correctly formatted single parameter
            return commands[key].ParameterCount == parametersCount;
        }

        private static bool ValidateParameters(string input)
        {
            // Check if the command requires parameters or if the input has correctly formatted single parameter
            return Irt.SingleCheck(input);
        }
    }
}