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

namespace Interpreter
{
    internal static class IrtIfElseParser
    {
        /// <summary>
        /// Extracts the first if else.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Part of the string that contains the first if/else clause, even if stuff is nested</returns>
        internal static string ExtractFirstIfElse(string input)
        {
            var start = input.IndexOf("if(", StringComparison.OrdinalIgnoreCase);
            if (start == -1) return null; // No 'if' found

            var end = start;
            var braceCount = 0;
            var elseFound = false;
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
                else if (i + 4 <= input.Length && input.Substring(i, 4).Equals("else", StringComparison.OrdinalIgnoreCase) && braceCount == 0)
                {
                    elseFound = true;
                    var index = input.IndexOf("else", i, StringComparison.OrdinalIgnoreCase);
                    end = index + 4; // Update end to the end of else
                    break;
                }
            }

            // Look for the closing brace for the else block
            if (elseFound)
            {
                if (end == input.Length) return input.Substring(start, input.Length);

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

            // Return the extracted block
            return input.Substring(start, end - start + 1).Trim();
        }


        //TODO do not forget the values after the last }
        internal static IrtIfElseBlock Parse(IEnumerable<string> inputParts)
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

                                var innerIfElseBlock = new IrtIfElseBlock
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
