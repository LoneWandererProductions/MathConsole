using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
    internal sealed class IrtIfElseBlock
    {
        internal string Condition { get; init; }
        internal string IfClause { get; init; }
        internal string ElseClause { get; init; }

        public override string ToString()
        {
            return $"If({Condition}) {{ {IfClause} }} Else {{ {ElseClause} }}";
        }
    }

    public static class Experimental
    {
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

                if (IrtKernel.ContainsKeywordWithOpenParenthesis(token, "if"))
                {
                    var condition = IrtKernel.ExtractCondition(token, "if");
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
    }
}