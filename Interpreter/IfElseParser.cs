using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
    internal static class IfElseParser
    {
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
                    switch (token)
                    {
                        case "else":
                            inElse = true;
                            break;
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

                            var nestedIfElse = $"if({innerIfElseBlock.Condition}) {{ {innerIfElseBlock.IfClause} }} else {{ {innerIfElseBlock.ElseClause} }}";

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
