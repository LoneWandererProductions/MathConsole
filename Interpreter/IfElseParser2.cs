using System;
using System.Collections.Generic;

namespace Interpreter
{
    internal static class IfElseParser2
    {
        public static List<IfElseClaused> ParseIfElseClauses(string code)
        {
            var clauses = new List<IfElseClaused>();
            ParseIfElseClausesRecursively(code, clauses, 0);
            return clauses;
        }

        private static void ParseIfElseClausesRecursively(string code, List<IfElseClaused> clauses, int layer)
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
                //var (block, elsePosition) = ExtractFirstIfElseNew(codeFromIfIndex);

                if (string.IsNullOrWhiteSpace(block))
                {
                    break;
                }

                // Extract `if` and `else` clauses from the block
                string ifClause = block.Substring(0, elsePosition).Trim();
                string elseClause = elsePosition == block.Length ? null : block.Substring(elsePosition).Trim();

                // Save the extracted block with the current layer
                var ifElseClause = new IfElseClaused
                {
                    Parent = code,
                    IfClause = ifClause,
                    ElseClause = elseClause,
                    Layer = layer // Set the current depth level
                };
                clauses.Add(ifElseClause);

                // Remove outer `if` and `else` clauses before recursive parsing
                ifClause = RemoveOuterIfElse(ifClause);
                if (ifClause.Length > 0)
                {
                    ParseIfElseClausesRecursively(ifClause, clauses, layer + 1);
                }

                ifClause = RemoveOuterIfElse(elseClause);
                if (elseClause.Length > 0)
                {
                    ParseIfElseClausesRecursively(elseClause, clauses, layer + 1);
                }

                // Stop processing the current block and move on
                break; // Only process one `if-else` block per call
            }
        }

        // Method to remove the outermost if and else keywords
        private static string RemoveOuterIfElse(string code)
        {
            int ifIndex = code.IndexOf("if");
            if (ifIndex == 0) // Only remove if the code starts with "if"
            {
                // Remove the "if" keyword and the outermost braces
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

        public class IfElseClaused
        {
            public string Parent { get; set; }
            public string IfClause { get; set; }
            public string ElseClause { get; set; }
            public int Layer { get; set; } // Depth level of the `if-else` structure
        }
    }
}
