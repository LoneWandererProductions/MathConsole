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
        /// Parses the given code string to extract all If-Else clauses.
        /// </summary>
        /// <param name="code">The code string containing If-Else clauses.</param>
        /// <returns>A list of IfElseClause objects representing each If-Else clause found.</returns>
        public static List<IfElseClause> ParseIfElseClauses(string code)
        {
            var clauses = new List<IfElseClause>();
            ParseIfElseClausesRecursively(code, clauses, 0);
            return clauses;
        }

        /// <summary>
        /// Recursively parses If-Else clauses from the code string.
        /// </summary>
        /// <param name="code">The code string containing If-Else clauses.</param>
        /// <param name="clauses">The list to which found If-Else clauses are added.</param>
        /// <param name="layer">The current nesting layer of the If-Else clauses.</param>
        private static void ParseIfElseClausesRecursively(string code, List<IfElseClause> clauses, int layer)
        {
            while (true)
            {
                var ifIndex = FindFirstIfIndex(code, "if");
                if (ifIndex == -1)
                    break;

                var codeFromIfIndex = code.Substring(ifIndex);
                var (block, elsePosition) = ExtractFirstIfElse(codeFromIfIndex);

                if (string.IsNullOrWhiteSpace(block)) break;

                var ifElseClause = CreateIfElseClause(code, block, elsePosition, layer);
                clauses.Add(ifElseClause);

                var innerIfClause = ExtractInnerIfElse(ifElseClause.IfClause);
                if (innerIfClause.Length > 0) ParseIfElseClausesRecursively(innerIfClause, clauses, layer + 1);

                var innerElseClause = ExtractInnerIfElse(ifElseClause.ElseClause);
                if (innerElseClause.Length > 0) ParseIfElseClausesRecursively(innerElseClause, clauses, layer + 1);

                break;
            }
        }

        /// <summary>
        /// Creates an IfElseClause object from the extracted If-Else block.
        /// </summary>
        /// <param name="code">The original code string.</param>
        /// <param name="block">The extracted block containing If-Else code.</param>
        /// <param name="elsePosition">The position of the 'else' keyword in the block.</param>
        /// <param name="layer">The nesting layer of the If-Else clause.</param>
        /// <returns>An IfElseClause object representing the parsed If-Else clause.</returns>
        private static IfElseClause CreateIfElseClause(string code, string block, int elsePosition, int layer)
        {
            var ifClause = block.Substring(0, elsePosition).Trim();
            var elseClause = elsePosition == -1 ? null : block.Substring(elsePosition).Trim();

            return new IfElseClause
            {
                Parent = code,
                IfClause = ifClause,
                ElseClause = elseClause,
                Layer = layer
            };
        }

        /// <summary>
        /// Removes the outermost If-Else structure from the given code string.
        /// </summary>
        /// <param name="code">The code string containing an If-Else structure.</param>
        /// <returns>The code string with the outer If-Else structure removed.</returns>
        private static string ExtractInnerIfElse(string code)
        {
            if (!code.StartsWith("if", StringComparison.OrdinalIgnoreCase)) return code;

            var openBraceIndex = code.IndexOf('{');
            var closeBraceIndex = FindBlockEnd(code, openBraceIndex);

            return closeBraceIndex != -1
                ? code.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1).Trim()
                : code;
        }

        /// <summary>
        /// Finds the index of the closing brace that matches the opening brace starting at the given index.
        /// </summary>
        /// <param name="code">The code string.</param>
        /// <param name="start">The index of the opening brace.</param>
        /// <returns>The index of the matching closing brace, or -1 if not found.</returns>
        private static int FindBlockEnd(string code, int start)
        {
            var braceCount = 0;

            for (var i = start; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case '{':
                        braceCount++;
                        break;
                    case '}':
                        braceCount--;
                        if (braceCount == 0) return i;

                        break;
                }
            }

            return -1;
        }

        /// <summary>
        /// Builds a categorized dictionary of commands from the given input string.
        /// </summary>
        /// <param name="input">The input string containing command definitions.</param>
        /// <returns>A CategorizedDictionary containing the parsed commands.</returns>
        internal static CategorizedDictionary<int, string> BuildCommand(string input)
        {
            // Remove unnecessary characters from the input string
            input = Irt.RemoveLastOccurrence(input, IrtConst.AdvancedClose);
            input = Irt.RemoveFirstOccurrence(input, IrtConst.AdvancedOpen);
            input = input.Trim();

            var formattedBlocks = new List<string>();
            var keepParsing = true;

            while (keepParsing)
            {
                var ifIndex = FindFirstIfIndex(input, "if");
                if (ifIndex == -1)
                {
                    keepParsing = false;
                    if (!string.IsNullOrWhiteSpace(input))
                        formattedBlocks.Add(input.Trim());
                }
                else
                {
                    var beforeIf = input.Substring(0, ifIndex).Trim();
                    if (!string.IsNullOrWhiteSpace(beforeIf)) formattedBlocks.Add(beforeIf);

                    input = input.Substring(ifIndex);
                    var (ifElseBlock, elsePosition) = ExtractFirstIfElse(input);

                    if (elsePosition == -1)
                    {
                        formattedBlocks.Add(ifElseBlock.Trim());
                    }
                    else
                    {
                        var ifBlock = input.Substring(0, elsePosition).Trim();
                        var elseBlock = input.Substring(elsePosition, ifElseBlock.Length - elsePosition).Trim();
                        formattedBlocks.Add(ifBlock);
                        formattedBlocks.Add(elseBlock);
                    }

                    input = input.Substring(ifElseBlock.Length).Trim();
                }
            }

            var commandRegister = new CategorizedDictionary<int, string>();
            var commandIndex = 0;

            foreach (var block in formattedBlocks)
            {
                var keywordIndex = GetCommandIndex(block, IrtConst.InternContainerCommands);

                switch (keywordIndex)
                {
                    case 0:
                    case 1:
                        commandRegister.Add(IrtConst.InternContainerCommands[keywordIndex].Command, commandIndex, block);
                        break;

                    default:
                        foreach (var trimmedSubCommand in Irt.SplitParameter(block, IrtConst.NewCommand)
                            .Select(subCommand => subCommand.Trim()))
                        {
                            keywordIndex = GetCommandIndex(trimmedSubCommand, IrtConst.InternContainerCommands);
                            commandIndex++;

                            switch (keywordIndex)
                            {
                                case 2:
                                case 3:
                                    commandRegister.Add(IrtConst.InternContainerCommands[keywordIndex].Command, commandIndex, trimmedSubCommand);
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
        /// Parses the given input parts to create an IrtIfElseBlock.
        /// </summary>
        /// <param name="inputParts">The input parts to parse.</param>
        /// <returns>An IrtIfElseBlock representing the parsed If-Else structure.</returns>
        internal static IrtIfElseBlock Parse(IEnumerable<string> inputParts)
        {
            var stack = new Stack<(string Condition, StringBuilder IfClause, StringBuilder ElseClause, bool InElse)>();
            var currentIfClause = new StringBuilder();
            var currentElseClause = new StringBuilder();
            var inElse = false;
            string currentCondition = null;

            IrtIfElseBlock innerIfElseBlock = null;

            var tokens = inputParts.SelectMany(Tokenize).ToList();

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i].Trim();

                if (ContainsKeywordWithOpenParen(token, "if"))
                {
                    var condition = ExtractCondition(token, "if");
                    stack.Push((currentCondition, currentIfClause, currentElseClause, inElse));
                    currentCondition = condition;
                    currentIfClause = new StringBuilder();
                    currentElseClause = new StringBuilder();
                    inElse = false;

                    var containsStandaloneElse = tokens.Any(part =>
                        part.Trim().Equals("else", StringComparison.OrdinalIgnoreCase) ||
                        part.Trim().Contains("}else{", StringComparison.OrdinalIgnoreCase));

                    if (!containsStandaloneElse)
                    {
                        while (++i < tokens.Count)
                        {
                            var nextToken = tokens[i].Trim();
                            if (nextToken == "{")
                                continue;

                            if (nextToken == "}")
                                break;

                            currentIfClause.Append($"{nextToken} ");
                        }

                        return new IrtIfElseBlock
                        {
                            Condition = currentCondition,
                            IfClause = currentIfClause.ToString().Trim(),
                            ElseClause = null
                        };
                    }
                }
                else if (token.Equals("else", StringComparison.OrdinalIgnoreCase) ||
                         token.Contains("}else{", StringComparison.OrdinalIgnoreCase))
                {
                    inElse = true;

                    if (!token.Contains("}else{")) continue;

                    token = token.Replace("}else{", string.Empty).Trim();

                    if (!string.IsNullOrEmpty(token)) currentElseClause.Append($"{token} ");
                }
                else
                {
                    switch (token.ToUpperInvariant())
                    {
                        case "}":
                            if (!inElse)
                            {
                                break;
                            }
                            else
                            {
                                if (stack.Count == 0)
                                    throw new InvalidOperationException("Invalid input string: unmatched closing brace");

                                var (parentCondition, parentIfPart, parentElsePart, parentInElse) = stack.Pop();

                                innerIfElseBlock = new IrtIfElseBlock
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
                            }

                        case "{":
                            break;

                        default:
                            if (inElse)
                                currentElseClause.Append($"{token} ");
                            else
                                currentIfClause.Append($"{token} ");
                            break;
                    }
                }
            }

            if (innerIfElseBlock != null) return innerIfElseBlock;

            throw new InvalidOperationException("Invalid input string");
        }

        /// <summary>
        /// Extracts the first If-Else block and the position of the 'else' keyword from the input string.
        /// </summary>
        /// <param name="input">The input string containing the If-Else structure.</param>
        /// <returns>A tuple containing the extracted If-Else block and the position of the 'else' keyword.</returns>
        internal static (string block, int elsePosition) ExtractFirstIfElse(string input)
        {
            var start = input.IndexOf("if(", StringComparison.OrdinalIgnoreCase);
            if (start == -1) return (null, -1);

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
                    end = index + 4;
                    break;
                }
            }

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

            return (input.Substring(start, end - start + 1).Trim(), elsePosition);
        }

        /// <summary>
        /// Tokenize the input string into individual tokens for parsing.
        /// </summary>
        /// <param name="input">The input string to tokenize.</param>
        /// <returns>A collection of tokens extracted from the input string.</returns>
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
                        if (sb.Length > 0)
                        {
                            cache = sb.ToString();
                            if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);
                            sb.Clear();
                        }

                        tokens.Add(input[i].ToString());
                        break;

                    default:
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
                            i = endIdx;
                        }
                        else if (input.Substring(i).StartsWith("else", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (sb.Length > 0)
                            {
                                cache = sb.ToString();
                                if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);
                                sb.Clear();
                            }

                            tokens.Add("else");
                            i += 3;
                        }
                        else
                        {
                            sb.Append(input[i]);
                        }

                        break;
                }
            }

            cache = sb.ToString();
            if (!string.IsNullOrWhiteSpace(cache)) tokens.Add(cache);

            return tokens;
        }

        /// <summary>
        /// Determines the command index based on the input string and a dictionary of commands.
        /// </summary>
        /// <param name="input">The input string to check against commands.</param>
        /// <param name="commands">A dictionary of commands to match against.</param>
        /// <returns>The index of the matching command, or an error code if no match is found.</returns>
        private static int GetCommandIndex(string input, Dictionary<int, InCommand> commands)
        {
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

            foreach (var (key, command) in commands)
                if (string.Equals(input, command.Command, StringComparison.OrdinalIgnoreCase))
                    return key;

            return IrtConst.Error;
        }

        /// <summary>
        /// Extracts the condition from an 'if' statement.
        /// </summary>
        /// <param name="input">The input string containing the 'if' statement.</param>
        /// <param name="keyword">The keyword to remove from the start of the string.</param>
        /// <returns>The extracted condition string.</returns>
        private static string ExtractCondition(string input, string keyword)
        {
            if (input.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                input = input.Substring(keyword.Length).Trim();

            if (input.StartsWith("(", StringComparison.Ordinal)) input = input.Substring(1).Trim();
            if (input.EndsWith(")", StringComparison.Ordinal)) input = input.Substring(0, input.Length - 1).Trim();

            return input;
        }

        /// <summary>
        /// Finds the index of the first occurrence of the given keyword followed by an open parenthesis.
        /// </summary>
        /// <param name="input">The input string to search within.</param>
        /// <param name="keyword">The keyword to find.</param>
        /// <returns>The index of the keyword if found, or -1 if not found.</returns>
        private static int FindFirstIfIndex(string input, string keyword)
        {
            var position = input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);

            while (position != -1)
            {
                var openParenIndex = position + keyword.Length;
                while (openParenIndex < input.Length && char.IsWhiteSpace(input[openParenIndex])) openParenIndex++;

                if (openParenIndex < input.Length && input[openParenIndex] == IrtConst.BaseOpen)
                    return position;

                position = input.IndexOf(keyword, position + 1, StringComparison.OrdinalIgnoreCase);
            }

            return -1;
        }

        /// <summary>
        /// Checks if the input string contains a keyword followed by an open parenthesis.
        /// </summary>
        /// <param name="input">The input string to check.</param>
        /// <param name="keyword">The keyword to look for.</param>
        /// <returns>True if the keyword followed by an open parenthesis is found, otherwise false.</returns>
        private static bool ContainsKeywordWithOpenParen(string input, string keyword)
        {
            return FindFirstIfIndex(input, keyword) != -1;
        }
    }
}
