/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/Irt.cs
 * PURPOSE:     Checks multiple logic errors
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable ArrangeBraces_foreach

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Interpreter
{
    /// <summary>
    ///     The Irt class.
    /// </summary>
    internal static class Irt
    {
        /// <summary>
        ///     Validates the parameters. Parameter Count
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parametersCount">Count of parameter</param>
        /// <param name="commands">The commands.</param>
        /// <returns>If Parameters are correct</returns>
        internal static bool ValidateParameters(int key, int parametersCount,
            IReadOnlyDictionary<int, InCommand> commands)
        {
            // Check if the command requires parameters or if the input has correctly formatted single parameter
            return commands[key].ParameterCount == parametersCount;
        }

        /// <summary>
        ///     Validates the parameters. Parenthesis.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>if the input has correctly formatted parenthesis</returns>
        internal static bool ValidateParameters(string input)
        {
            // if the input has correctly formatted parenthesis
            return SingleCheck(input);
        }

        /// <summary>
        ///     Checks if the input string has balanced parentheses of a single type.
        /// </summary>
        /// <param name="input">Input string to check.</param>
        /// <returns>True if parentheses are balanced, false otherwise.</returns>
        /// <example>
        ///     <code>
        /// bool result = Irt.SingleCheck("(a + b) * c");
        /// // result is true
        /// </code>
        /// </example>
        internal static bool SingleCheck(string input)
        {
            // Index of the currently open parentheses:
            var parentheses = new Stack<int>();

            foreach (var chr in input)
            {
                if (chr == IrtConst.BaseOpen)
                {
                    parentheses.Push(0); // Add index to stack
                }
                // Check if the 'chr' is a close parenthesis, and get its index:
                else
                {
                    if (chr != IrtConst.BaseClose)
                    {
                        continue;
                    }

                    // Return 'false' if the stack is empty or if the currently
                    if (parentheses.Count == 0)
                    {
                        return false;
                    }

                    // open parenthesis is not paired with the 'chr':
                    if (parentheses.Pop() != 0)
                    {
                        return false;
                    }
                }
            }

            // Return 'true' if there is no open parentheses, and 'false' - otherwise:
            return parentheses.Count == 0;
        }

        /// <summary>
        ///     Checks if the input string has balanced parentheses of multiple types.
        /// </summary>
        /// <param name="input">Input string to check.</param>
        /// <param name="openParenthesis">Array of opening parentheses characters.</param>
        /// <param name="closeParenthesis">Array of closing parentheses characters.</param>
        /// <returns>True if parentheses are balanced, false otherwise.</returns>
        internal static bool CheckMultiple(string input, char[] openParenthesis, char[] closeParenthesis)
        {
            //Open and close parentheses arrays are not the same length.
            if (openParenthesis.Length != closeParenthesis.Length)
            {
                return false;
            }

            // Index of the currently open parentheses:
            var parentheses = new Stack<int>();

            foreach (var chr in input)
            {
                int index;

                // Check if the 'chr' is an open parenthesis, and get its index:
                if ((index = Array.IndexOf(openParenthesis, chr)) != -1)
                {
                    parentheses.Push(index); // Add index to stack
                }
                // Check if the 'chr' is a close parenthesis, and get its index:
                else
                {
                    if ((index = Array.IndexOf(closeParenthesis, chr)) == -1)
                    {
                        continue;
                    }

                    // Return 'false' if the stack is empty or if the currently
                    // open parenthesis is not paired with the 'chr':
                    if (parentheses.Count == 0 || parentheses.Pop() != index)
                    {
                        return false;
                    }
                }
            }

            // Return 'true' if there is no open parentheses, and 'false' - otherwise:
            return parentheses.Count == 0;
        }

        /// <summary>
        ///     Removes the outermost parentheses from the input string if well-formed.
        /// </summary>
        /// <param name="input">Last bit of string</param>
        /// <param name="openClause">The open clause.</param>
        /// <param name="closeClause">The close clause.</param>
        /// <returns>
        ///     Cleaned String or Error Message
        /// </returns>
        internal static string RemoveParenthesis(string input, char openClause, char closeClause)
        {
            //no Parenthesis? okay we still try to handle it, might be a command with zero parameters
            if (!input.Contains(IrtConst.BaseOpen) && !input.Contains(closeClause))
            {
                return input;
            }

            // Ensure the string starts with the openClause and ends with the closeClause
            return StartsAndEndsWith(input, openClause, closeClause)
                ? input.Substring(1, input.Length - 2).Trim()
                : IrtConst.ParenthesisError;
        }

        /// <summary>
        ///     Removes the first occurrence of the specified symbol from the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="symbol">The symbol to remove.</param>
        /// <returns>The modified string.</returns>
        internal static string RemoveFirstOccurrence(string input, char symbol)
        {
            return input.Remove(input.IndexOf(symbol.ToString(), StringComparison.Ordinal), 1);
        }

        /// <summary>
        ///     Removes the last symbol.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="symbol">The symbol.</param>
        /// <returns>The modified string.</returns>
        internal static string RemoveLastOccurrence(string input, char symbol)
        {
            return input.Substring(0, input.LastIndexOf(symbol.ToString(), StringComparison.Ordinal));
        }

        /// <summary>
        ///     Removes specified part of String
        /// </summary>
        /// <param name="remove">Keyword to remove</param>
        /// <param name="target">target string</param>
        /// <returns>Parameter Part without the Keyword at the front</returns>
        internal static string RemoveWord(string remove, string target)
        {
            // Find the index of remove in target, ignoring case
            var index = target.IndexOf(remove, StringComparison.OrdinalIgnoreCase);

            // If remove is found, remove it from target
            if (index >= 0)
            {
                target = target.Remove(index, remove.Length);
            }

            return target.Trim();
        }

        /// <summary>
        ///     For External Commands
        /// </summary>
        /// <param name="input">Command String</param>
        /// <param name="com">Command Register</param>
        /// <returns>Id of Register used, if nothing was found, return -1.</returns>
        internal static int CheckForKeyWord(string input, Dictionary<int, InCommand> com)
        {
            //just for the compare, make it to upper do not change the input string
            input = input.ToUpperInvariant();

            if (input.Contains(IrtConst.AdvancedOpen))
            {
                var index = input.IndexOf(IrtConst.AdvancedOpen);

                if (index >= 0)
                {
                    input = input[..index];
                    input = input.Trim();
                }
            }
            else if (input.Contains(IrtConst.BaseOpen))
            {
                var index = input.IndexOf(IrtConst.BaseOpen);

                if (index >= 0)
                {
                    input = input[..index];
                    input = input.Trim();
                }
            }

            foreach (var (key, inCommand) in com)
            {
                if (string.Equals(input.ToUpperInvariant(), inCommand.Command.ToUpperInvariant(),
                        StringComparison.Ordinal))
                {
                    return
                        key;
                }
            }

            return IrtConst.Error;
        }

        /// <summary>
        ///     Get the Parameter as list
        /// </summary>
        /// <param name="parameterPart">Parameter String</param>
        /// <param name="splitter">the char we split</param>
        /// <returns>Splits Parameter Part by Splitter</returns>
        internal static List<string> SplitParameter(string parameterPart, char splitter)
        {
            var lst = parameterPart.Split(splitter).ToList();
            var paramLst = new List<string>(lst.Count);

            paramLst.AddRange(lst.Select(param => param.Trim()));

            // remove empty trash
            paramLst = paramLst.Where(element => !string.IsNullOrEmpty(element)).ToList();

            //remove all empty Parameters
            return (from item in paramLst
                let result = Regex.Replace(item, IrtConst.RegexRemoveWhiteSpace, string.Empty)
                where result != IrtConst.EmptyParameter
                select item).ToList();
        }

        /// <summary>
        ///     Makes the Parenthesis well formed e.g.
        ///     (),() and not like () ,   (), also handles all cases for {}
        ///     Not yet in use
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>Well formed string with Parenthesis</returns>
        internal static string WellFormedParenthesis(string input)
        {
            input = input.Trim();
            var regex = Regex.Replace(input, IrtConst.RegexParenthesisWellFormedPatternLeft, string.Empty);
            return Regex.Replace(regex, IrtConst.RegexParenthesisWellFormedPatternRight, string.Empty);
        }

        /// <summary>
        ///     Check the overload and the Parameter Count
        ///     if count is positive count and parameter must be equal.
        ///     if count is negative the parameter count must be equal and or bigger
        /// </summary>
        /// <param name="command">The command Keyword</param>
        /// <param name="count">The count of Parameters</param>
        /// <param name="commands">The Command Dictionary</param>
        /// <returns>
        ///     The Command Id, identical, if there is no overload, new id, if there is an overload, null if something went
        ///     wrong <see cref="int" />.
        /// </returns>
        internal static int? CheckOverload(string command, int count, Dictionary<int, InCommand> commands)
        {
            foreach (var (key, value) in commands.Where(comm => command == comm.Value.Command))
            {
                if (value.ParameterCount < 0)
                {
                    if (count >= Math.Abs(value.ParameterCount)) return key;
                }
                else
                {
                    if (value.ParameterCount == count) return key;
                }
            }

            return null;
        }

        /// <summary>
        ///     Processes the parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="commands">The commands in use.</param>
        /// <returns>return Parameter</returns>
        internal static (int Status, string Parameter) GetParameters(string input, int key,
            IReadOnlyDictionary<int, InCommand> commands)
        {
            var command = commands[key].Command.ToUpperInvariant();
            var parameterPart = RemoveWord(command, input);

            return parameterPart.StartsWith(IrtConst.AdvancedOpen)
                ? (IrtConst.BatchCommand, parameterPart)
                : (IrtConst.ParameterCommand,
                    RemoveParenthesis(parameterPart, IrtConst.BaseOpen, IrtConst.BaseClose));
        }

        /// <summary>
        ///     Checks if the input string follows the format of "command(label)" with optional whitespace and case insensitivity.
        /// </summary>
        /// <param name="input">The input string to check.</param>
        /// <param name="command">The expected command.</param>
        /// <param name="label">The expected label.</param>
        /// <returns>True if the string matches the format; otherwise, false.</returns>
        internal static bool CheckFormat(string input, string command, string label)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // Trim the input to remove leading and trailing whitespace
            var trimmedInput = input.Trim();

            // Convert to upper case for case-insensitive comparison
            var upperInput = trimmedInput.ToUpperInvariant();
            var upperCommand = command.ToUpperInvariant();
            var upperLabel = label.ToUpperInvariant();

            // Check if it starts with "command(" and ends with ")"
            var start = upperCommand + IrtConst.BaseOpen;
            var end = IrtConst.BaseClose.ToString();

            if (!upperInput.StartsWith(start, StringComparison.Ordinal) ||
                !upperInput.EndsWith(end, StringComparison.Ordinal))
                return false;

            // Extract the content within the parentheses
            var contentStartIndex = upperCommand.Length + 1;
            var contentLength = upperInput.Length - contentStartIndex - 1;
            var content = upperInput.Substring(contentStartIndex, contentLength).Trim();

            // Check if the content matches the expected label
            return content == upperLabel;
        }

        internal static IfElseBlock HandleIfElseBlock(List<string> commands, int currentPosition)
        {
            var block = new IfElseBlock
            {
            };

            return block;
        }


        /// <summary>
        ///     Checks if the string starts and ends with the specified characters.
        /// </summary>
        /// <param name="input">Input string to check.</param>
        /// <param name="start">Expected starting character.</param>
        /// <param name="end">Expected ending character.</param>
        /// <returns>True if the string starts with 'start' and ends with 'end', false otherwise.</returns>
        private static bool StartsAndEndsWith(string input, char start, char end)
        {
            return input.Length > 1 && input[0] == start && input[^1] == end;
        }
    }
}