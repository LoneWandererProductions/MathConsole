/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     Interpreter
* FILE:        Interpreter/IrtIfElseParser.cs
* PURPOSE:     The if else parser and a piece of work, not really happy about it.
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

using ExtendedSystemObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
    internal static class IrtIfElseParser
    {
        public static List<IfElseClause> ParseIfElseClauses(string code)
        {
            var clauses = new List<IfElseClause>();
            ParseIfElseClausesRecursively(code, clauses, 0);
            return clauses;
        }

        private static void ParseIfElseClausesRecursively(string code, List<IfElseClause> clauses, int layer)
        {
            while (true)
            {
                // Find the next 'if' statement in the current substring
                int ifIndex = IrtIfElseParser.FindFirstIfIndex(code);
                if (ifIndex == -1)
                    break;

                // Extract the substring starting from the found 'if' index
                var codeFromIfIndex = code.Substring(ifIndex);
                var (block, elsePosition) = IrtIfElseParser.ExtractFirstIfElse(codeFromIfIndex);

                if (string.IsNullOrWhiteSpace(block))
                {
                    break;
                }

                // Extract `if` and `else` clauses from the block
                string ifClause = block.Substring(0, elsePosition).Trim();
                string elseClause = elsePosition == -1 ? null : block.Substring(elsePosition).Trim();

                // Save the extracted block with the current layer
                var ifElseClause = new IfElseClause
                {
                    Parent = code,
                    IfClause = ifClause,
                    ElseClause = elseClause,
                    Layer = layer // Set the current depth level
                };
                clauses.Add(ifElseClause);

                // Remove outer `if` and `else` clauses before recursive parsing
                string innerIfClause = RemoveOuterIfElse(ifClause);
                if (innerIfClause.Length > 0)
                {
                    ParseIfElseClausesRecursively(innerIfClause, clauses, layer + 1);
                }

                string innerElseClause = RemoveOuterIfElse(elseClause);
                if (innerElseClause.Length > 0)
                {
                    ParseIfElseClausesRecursively(innerElseClause, clauses, layer + 1);
                }

                // Stop processing the current block and move on
                break; // Only process one `if-else` block per call
            }
        }

        // Method to remove the outermost if and else keywords
        private static string RemoveOuterIfElse(string code)
        {
            if (code.StartsWith("if"))
            {
                int openBraceIndex = code.IndexOf('{');
                int closeBraceIndex = FindBlockEnd(code, openBraceIndex);
                if (closeBraceIndex != -1)
                {
                    return code.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1).Trim();
                }
            }
            return code;
        }

        // Finds the index of the closing brace that matches the opening brace at the given start index.
        public static int FindBlockEnd(string code, int start)
        {
            int braceCount = 0;

            for (int i = start; i < code.Length; i++)
            {
                if (code[i] == '{')
                {
                    braceCount++;
                }
                else if (code[i] == '}')
                {
                    braceCount--;
                    if (braceCount == 0)
                    {
                        return i; // Return the index of the closing brace
                    }
                }
            }

            return -1; // No matching closing brace found
        }

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
            {
                if (token.StartsWith("if(", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Start a new if block
                    var condition = token.Substring(3, token.Length - 4); // Extract condition
                    stack.Push((currentCondition, currentIfClause, currentElseClause, inElse));
                    currentCondition = condition;
                    currentIfClause = new StringBuilder();
                    currentElseClause = new StringBuilder();
                    inElse = false;
                }
                else
                {
                    if (!string.Equals(token, "ELSE", StringComparison.InvariantCultureIgnoreCase))
                    {
                        switch (token.ToUpperInvariant())
                        {
                            case "}" when !inElse:
                                // End of an if block, continue
                                break;

                            case "}" when inElse:
                                // End of else block
                                if (stack.Count == 0)
                                    throw new InvalidOperationException("Invalid input string: unmatched closing brace");

                                var (parentCondition, parentIfPart, parentElsePart, parentInElse) = stack.Pop();

                                var innerIfElseBlock = new IrtIfElseBlock
                                {
                                    Condition = currentCondition,
                                    IfClause = currentIfClause.ToString().Trim(),
                                    ElseClause = currentElseClause.ToString().Trim()
                                };

                                var nestedIfElse =
                                    $"if({innerIfElseBlock.Condition}) {{ {innerIfElseBlock.IfClause} }} else {{ {innerIfElseBlock.ElseClause} }}";

                                if (parentCondition == null)
                                    return innerIfElseBlock;

                                if (parentInElse)
                                    parentElsePart.Append(nestedIfElse);
                                else
                                    parentIfPart.Append(nestedIfElse);

                                currentCondition = parentCondition;
                                currentIfClause = parentIfPart;
                                currentElseClause = parentElsePart;
                                inElse = parentInElse;
                                break;

                            case "{":
                                // Skip opening brace
                                break;

                            default:
                                // Append token to the appropriate clause
                                if (inElse)
                                    currentElseClause.Append($"{token.Trim()} ");
                                else
                                    currentIfClause.Append($"{token.Trim()} ");
                                break;
                        }
                    }
                    else
                    {
                        inElse = true;
                    }
                }
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
            {
                switch (input[i])
                {
                    case '{':
                    case '}':
                        // Add any buffered string if it exists
                        if (sb.Length > 0)
                        {
                            cache = sb.ToString();
                            if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);
                            sb.Clear();
                        }

                        // Add the brace as a separate token
                        tokens.Add(input[i].ToString());
                        break;

                    default:
                        // Handle 'if' statements
                        if (input.Substring(i).StartsWith("if(", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (sb.Length > 0)
                            {
                                cache = sb.ToString();
                                if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);
                                sb.Clear();
                            }

                            var endIdx = input.IndexOf(')', i);
                            if (endIdx == -1)
                                throw new InvalidOperationException("Invalid input string: missing closing parenthesis for 'if'");

                            tokens.Add(input.Substring(i, endIdx - i + 1));
                            i = endIdx; // Skip past the end of the 'if' condition
                        }
                        // Handle 'else'
                        else if (input.Substring(i).StartsWith("else", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (sb.Length > 0)
                            {
                                cache = sb.ToString();
                                if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);
                                sb.Clear();
                            }

                            tokens.Add("else");
                            i += 3; // Skip past "else"
                        }
                        // Accumulate characters into the buffer
                        else
                        {
                            sb.Append(input[i]);
                        }
                        break;
                }
            }

            // Final check to add any remaining buffered text
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
                           StringComparison.OrdinalIgnoreCase))
                    return
                        key;

            return IrtConst.Error;
        }

        /// <summary>
        ///     Finds the first if index.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>index of first if</returns>
        internal static int FindFirstIfIndex(string input)
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