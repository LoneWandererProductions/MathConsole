/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     Interpreter
* FILE:        Interpreter/IrtIfElseParser.cs
* PURPOSE:     The if else parser and a piece of work, not really happy about it.
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtendedSystemObjects;

namespace Interpreter
{
    internal static class IrtIfElseParser
    {
        /// <summary>
        ///     Builds the command.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Command Register</returns>
        internal static CategorizedDictionary<int, string> BuildCommand(string input)
        {
            input = Irt.RemoveLastOccurrence(input, IrtConst.AdvancedClose);
            input = Irt.RemoveFirstOccurrence(input, IrtConst.AdvancedOpen);
            input = input.Trim();

            var formattedBlocks = new List<string>();
            var keepParsing = true;

            while (keepParsing)
            {
                var ifIndex = FindFirstIfIndex(input);
                if (ifIndex == -1)
                {
                    keepParsing = false;
                    if (!string.IsNullOrWhiteSpace(input))
                        formattedBlocks.Add(input.Trim()); // Add remaining part as the last element
                }
                else
                {
                    // Add the part before the first 'if' to the list
                    var beforeIf = input.Substring(0, ifIndex).Trim();
                    if (!string.IsNullOrWhiteSpace(beforeIf)) formattedBlocks.Add(beforeIf);

                    input = input.Substring(ifIndex); // Update input to start from 'if'

                    var (ifElseBlock, elsePosition) = ExtractFirstIfElse(input);

                    if (elsePosition == -1)
                    {
                        formattedBlocks.Add(ifElseBlock.Trim()); // Add entire 'if' block if no 'else' found
                    }
                    else
                    {
                        var ifBlock = input.Substring(0, elsePosition).Trim(); // Part up to 'else'
                        var elseBlock =
                            input.Substring(elsePosition, ifElseBlock.Length - elsePosition)
                                .Trim(); // Part after 'else'
                        formattedBlocks.Add(ifBlock);
                        formattedBlocks.Add(elseBlock);
                    }

                    input = input.Substring(ifElseBlock.Length).Trim(); // Update input to remaining part
                }
            }

            var commandRegister = new CategorizedDictionary<int, string>();
            var commandIndex = 0;

            foreach (var block in formattedBlocks)
            {
                var keywordIndex = StartsWith(block, IrtConst.InternContainerCommands);

                switch (keywordIndex)
                {
                    case 0: // IF
                    case 1: // ELSE
                        commandRegister.Add(IrtConst.InternContainerCommands[keywordIndex].Command, commandIndex,
                            block);
                        break;

                    default:
                        foreach (var trimmedSubCommand in Irt.SplitParameter(block, IrtConst.NewCommand)
                                     .Select(subCommand => subCommand.Trim()))
                        {
                            keywordIndex = StartsWith(trimmedSubCommand, IrtConst.InternContainerCommands);
                            commandIndex++;

                            switch (keywordIndex)
                            {
                                case 2: // GOTO
                                case 3: // LABEL
                                    commandRegister.Add(IrtConst.InternContainerCommands[keywordIndex].Command,
                                        commandIndex, trimmedSubCommand);
                                    break;
                                default:
                                    if (!string.IsNullOrEmpty(trimmedSubCommand))
                                        commandRegister.Add("COMMAND", commandIndex, trimmedSubCommand);

                                    break;
                            }
                        }

                        break;
                }

                commandIndex++;
            }

            return commandRegister;
        }

        /// <summary>
        ///     Extracts the first if else.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Part of the string that contains the first if/else clause, even if stuff is nested</returns>
        internal static (string block, int elsePosition) ExtractFirstIfElse(string input)
        {
            var start = input.IndexOf("if(", StringComparison.OrdinalIgnoreCase);
            if (start == -1) return (null, -1); // No 'if' found

            var end = start;
            var braceCount = 0;
            var elseFound = false;
            var elsePosition = -1;
            var containsElse = input.IndexOf("else", StringComparison.OrdinalIgnoreCase) != -1;

            for (var i = start; i < input.Length; i++)
            {
                var cursor = input[i];

                if (cursor == '{')
                {
                    braceCount++;
                }
                else if (cursor == '}')
                {
                    braceCount--;
                    if (braceCount != 0) continue;
                    if (containsElse) continue;

                    end = i;
                    break;
                }
                else if (i + 4 <= input.Length &&
                         input.Substring(i, 4).Equals("else", StringComparison.OrdinalIgnoreCase) && braceCount == 0)
                {
                    elseFound = true;
                    elsePosition = i;
                    var index = input.IndexOf("else", i, StringComparison.OrdinalIgnoreCase);
                    end = index + 4; // Update end to the end of else
                    break;
                }
            }

            // Look for the closing brace for the else block
            if (elseFound)
            {
                if (end == input.Length) return (input.Substring(start, input.Length), elsePosition);

                for (var i = end; i < input.Length; i++)
                {
                    var cursor = input[i];

                    if (cursor == '{')
                    {
                        braceCount++;
                    }
                    else if (cursor == '}')
                    {
                        braceCount--;

                        if (braceCount != 0) continue;

                        end = i;
                        break;
                    }
                    else if (cursor == ';')
                    {
                        if (braceCount != 0) continue;

                        end = i;
                        break;
                    }
                }
            }

            // Return the extracted block and the else position
            return (input.Substring(start, end - start + 1).Trim(), elsePosition);
        }

        //TODO do not forget the values after the last }, will be removed now useless for me!
        internal static IrtIfElseBlock Parse(IEnumerable<string> inputParts)
        {
            var stack = new Stack<(string Condition, StringBuilder IfClause, StringBuilder ElseClause, bool InElse)>();
            var currentIfClause = new StringBuilder();
            var currentElseClause = new StringBuilder();
            var inElse = false;
            string currentCondition = null;

            foreach (var token in inputParts.SelectMany(Tokenize))
                if (token.StartsWith("if(", StringComparison.Ordinal))
                {
                    var condition = token.Substring(3, token.Length - 4);
                    stack.Push((currentCondition, currentIfClause, currentElseClause, inElse));
                    currentCondition = condition;
                    currentIfClause = new StringBuilder();
                    currentElseClause = new StringBuilder();
                    inElse = false;
                }
                else
                {
                    if (token.ToUpperInvariant() != IrtConst.InternalElse)
                        switch (token.ToUpperInvariant())
                        {
                            case "}" when !inElse:
                                // Skip the closing brace if not in else branch
                                break;
                            case "}" when inElse:
                            {
                                if (stack.Count == 0)
                                    throw new InvalidOperationException(
                                        "Invalid input string: unmatched closing brace");

                                var (parentCondition, parentIfPart, parentElsePart, parentInElse) = stack.Pop();

                                var innerIfElseBlock = new IrtIfElseBlock
                                {
                                    Condition = currentCondition,
                                    IfClause = currentIfClause.ToString().Trim(),
                                    ElseClause = currentElseClause.ToString().Trim()
                                };

                                var nestedIfElse =
                                    $"if({innerIfElseBlock.Condition}) {{ {innerIfElseBlock.IfClause} }} else {{ {innerIfElseBlock.ElseClause} }}";

                                if (parentCondition == null) return innerIfElseBlock;

                                if (parentInElse)
                                    parentElsePart.Append(nestedIfElse);
                                else
                                    parentIfPart.Append(nestedIfElse);

                                currentCondition = parentCondition;
                                currentIfClause = parentIfPart;
                                currentElseClause = parentElsePart;
                                inElse = parentInElse;
                                break;
                            }
                            case "{":
                                // Skip the opening brace
                                break;
                            default:
                            {
                                if (inElse)
                                    currentElseClause.Append($"{token.Trim()} ");
                                else
                                    currentIfClause.Append($"{token.Trim()} ");

                                break;
                            }
                        }
                    else
                        inElse = true;
                }

            throw new InvalidOperationException("Invalid input string");
        }

        private static IEnumerable<string> Tokenize(string input)
        {
            var tokens = new List<string>();
            var sb = new StringBuilder();
            input = input.Trim();
            string cache;

            for (var i = 0; i < input.Length; i++)
                switch (input[i])
                {
                    case IrtConst.AdvancedOpen:
                    case IrtConst.AdvancedClose:
                        // Add any buffered string if it exists
                        if (sb.Length > 0)
                        {
                            cache = sb.ToString();
                            if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);

                            sb.Clear();
                        }

                        tokens.Add(input[i].ToString());
                        continue;

                    default:
                        if (input.Substring(i).StartsWith("if(", StringComparison.Ordinal))
                        {
                            if (sb.Length > 0)
                            {
                                cache = sb.ToString();
                                if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);

                                sb.Clear();
                            }

                            var endIdx = input.IndexOf(')', i);
                            if (endIdx == -1)
                                throw new InvalidOperationException(
                                    "Invalid input string: missing closing parenthesis for 'if'");

                            tokens.Add(input.Substring(i, endIdx - i + 1));
                            i = endIdx;
                        }
                        else if (input.Substring(i).StartsWith("else", StringComparison.Ordinal))
                        {
                            if (sb.Length > 0)
                            {
                                cache = sb.ToString();
                                if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);

                                sb.Clear();
                            }

                            tokens.Add("else");
                            i += 3; // Skip 'else'
                        }
                        else
                        {
                            sb.Append(input[i]);
                        }

                        break;
                }

            // Final check to add any remaining token
            cache = sb.ToString();
            if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);

            return tokens;
        }

        /// <summary>
        ///     Starts the with.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="com">The COM.</param>
        /// <returns>Key of Command, else -1</returns>
        private static int StartsWith(string input, Dictionary<int, InCommand> com)
        {
            //just for the compare, make it to upper do not change the input string
            input = input.ToUpperInvariant();

            if (input.Contains(IrtConst.BaseOpen))
            {
                var index = input.IndexOf(IrtConst.BaseOpen);

                if (index >= 0) input = input.Substring(0, index).Trim();
            }

            if (input.Contains(IrtConst.AdvancedOpen))
            {
                var index = input.IndexOf(IrtConst.AdvancedOpen);

                if (index >= 0) input = input.Substring(0, index).Trim();
            }

            foreach (var (key, inCommand) in com)
                if (string.Equals(input.ToUpperInvariant(), inCommand.Command.ToUpperInvariant(),
                        StringComparison.Ordinal))
                    return
                        key;

            return IrtConst.Error;
        }

        /// <summary>
        ///     Finds the first if index.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>index of first if</returns>
        private static int FindFirstIfIndex(string input)
        {
            var position = input.IndexOf("if", StringComparison.OrdinalIgnoreCase);

            while (position != -1)
            {
                // Check if there is a '(' after "if" ignoring any whitespace
                var openParenIndex = position + 2; // "if" is two characters long
                while (openParenIndex < input.Length && char.IsWhiteSpace(input[openParenIndex])) openParenIndex++;

                if (openParenIndex < input.Length && input[openParenIndex] == IrtConst.BaseOpen) return position;

                // Find the next "if" in the string
                position = input.IndexOf("if", position + 1, StringComparison.OrdinalIgnoreCase);
            }

            return IrtConst.Error; // Not found or no valid "if" followed by '('
        }
    }
}