using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
    internal static class IfElseParser
    {
        public static string ExtractFirstIfElse(string input)
        {
            int start = input.IndexOf("if(");
            if (start == -1) return null; // No 'if' found

            int end = start;
            int braceCount = 0;
            bool elseFound = false;

            for (int i = start; i < input.Length; i++)
            {
                if (input[i] == '{')
                {
                    braceCount++;
                }
                else if (input[i] == '}')
                {
                    braceCount--;
                    if (braceCount == 0)
                    {
                        if (elseFound)
                        {
                            end = i;
                            break;
                        }
                    }
                }
                else if (input.Substring(i, 4) == "else" && braceCount == 0)
                {
                    elseFound = true;
                    end = i; // Update end to the start of else
                }
            }

            // Look for the closing brace for the else block
            if (elseFound)
            {
                for (int i = end; i < input.Length; i++)
                {
                    if (input[i] == '{')
                    {
                        braceCount++;
                    }
                    else if (input[i] == '}')
                    {
                        braceCount--;
                        if (braceCount == 0)
                        {
                            end = i;
                            break;
                        }
                    }
                }
            }

            // Return the extracted block
            return input.Substring(start, end - start + 1).Trim();
        }

        /// <summary>
        /// Finds if else block.
        /// Fails if there is another input string in the input string.
        /// </summary>
        /// <param name="input">The input parts.</param>
        /// <returns>The End Parameter of the End</returns>
        public static int FindLastClosingBracket(string input)
        {
            var stack = new Stack<int>();
            var lastClosingBracketPos = -1;
            var outerIfCount = 0;

            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];

                switch (c)
                {
                    case '{':
                    {
                        if (outerIfCount == 0)
                        {
                            // We're entering an outer if block
                        }

                        stack.Push(i);
                        break;
                    }
                    case '}':
                    {
                        if (stack.Count > 0)
                        {
                            lastClosingBracketPos = i;
                            stack.Pop();

                            // Check if we closed an outer if
                            if (stack.Count == 0)
                            {
                                outerIfCount++; // Count complete outer if blocks
                            }
                        }

                        break;
                    }
                }

                // Reset outer count if we find another if without braces
                if (i < input.Length - 2 && input.Substring(i, 2) == "if")
                {
                    outerIfCount = 0; // Reset if another outer if is found
                }
            }

            return lastClosingBracketPos;
        }

        //TODO do not forget the values after the last }
        internal static IfElseBlock Parse(IEnumerable<string> inputParts)
        {
            var stack = new Stack<(string Condition, StringBuilder IfClause, StringBuilder ElseClause, bool InElse)>();
            var currentIfClause = new StringBuilder();
            var currentElseClause = new StringBuilder();
            var inElse = false;
            string currentCondition = null;

            foreach (var token in inputParts.SelectMany(Tokenize))
            {
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
                    {
                        switch (token.ToUpperInvariant())
                        {
                            case "}" when !inElse:
                                // Skip the closing brace if not in else branch
                                break;
                            case "}" when inElse:
                            {
                                if (stack.Count == 0)
                                {
                                    throw new InvalidOperationException("Invalid input string: unmatched closing brace");
                                }

                                var (parentCondition, parentIfPart, parentElsePart, parentInElse) = stack.Pop();

                                var innerIfElseBlock = new IfElseBlock
                                {
                                    Condition = currentCondition,
                                    IfClause = currentIfClause.ToString().Trim(),
                                    ElseClause = currentElseClause.ToString().Trim()
                                };

                                var nestedIfElse =
                                    $"if({innerIfElseBlock.Condition}) {{ {innerIfElseBlock.IfClause} }} else {{ {innerIfElseBlock.ElseClause} }}";

                                if (parentCondition == null)
                                {
                                    return innerIfElseBlock;
                                }

                                if (parentInElse)
                                {
                                    parentElsePart.Append(nestedIfElse);
                                }
                                else
                                {
                                    parentIfPart.Append(nestedIfElse);
                                }

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
                                {
                                    currentElseClause.Append($"{token.Trim()} ");
                                }
                                else
                                {
                                    currentIfClause.Append($"{token.Trim()} ");
                                }

                                break;
                            }
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
                    case IrtConst.AdvancedOpen:
                    case IrtConst.AdvancedClose:
                        // Add any buffered string if it exists
                        if (sb.Length > 0)
                        {
                            cache = sb.ToString();
                            if (!string.IsNullOrWhiteSpace(cache))
                            {
                                tokens.Add(cache);
                            }

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
                                if (!string.IsNullOrWhiteSpace(cache))
                                {
                                    tokens.Add(cache);
                                }

                                sb.Clear();
                            }

                            var endIdx = input.IndexOf(')', i);
                            if (endIdx == -1)
                            {
                                throw new InvalidOperationException("Invalid input string: missing closing parenthesis for 'if'");
                            }

                            tokens.Add(input.Substring(i, endIdx - i + 1));
                            i = endIdx;
                        }
                        else if (input.Substring(i).StartsWith("else", StringComparison.Ordinal))
                        {
                            if (sb.Length > 0)
                            {
                                cache = sb.ToString();
                                if (!string.IsNullOrWhiteSpace(cache))
                                {
                                    tokens.Add(cache);
                                }

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
            }

            // Final check to add any remaining token
            cache = sb.ToString();
            if (!string.IsNullOrWhiteSpace(cache))
            {
                tokens.Add(cache);
            }

            return tokens;
        }
    }
}
