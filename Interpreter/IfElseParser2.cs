using System;
using System.Collections.Generic;

namespace Interpreter
{
    internal static class IfElseParser2
    {
        public static List<IfElseClause> ParseIfElseClauses(string code)
        {
            var clauses = new List<IfElseClause>();
            ParseIfElseClausesRecursively(code, clauses, 0);
            return clauses;
        }

        private static void ParseIfElseClausesRecursively(string code, List<IfElseClause> clauses, int startIndex = 0)
        {
            while (true)
            {
                // Find the next 'if' statement in the code starting from startIndex
                var ifIndex = IrtIfElseParser.FindFirstIfIndex(code.Substring(startIndex));
                if (ifIndex == -1)
                    break;

                // Adjust ifIndex to the original code's index
                ifIndex += startIndex;

                // Extract the outermost if-else block
                var codeFromIfIndex = code.Substring(ifIndex);
                var (block, elsePosition) = IrtIfElseParser.ExtractFirstIfElse(codeFromIfIndex);

                // Calculate the correct index for `elsePosition`
                var actualElseIndex = elsePosition == -1 ? -1 : elsePosition + ifIndex;

                // Save the extracted outer block and its positions
                var ifElseClause = new IfElseClause
                {
                    IfIndex = ifIndex,
                    Block = block,
                    ElseIndex = actualElseIndex
                };
                clauses.Add(ifElseClause);

                // Extract the code within the `if` block
                int ifBlockStart = block.IndexOf('{') + 1;
                int ifBlockEnd = block.LastIndexOf('}');

                if (ifBlockStart > 0 && ifBlockEnd > ifBlockStart) // Ensure valid indices
                {
                    string ifBlockContent = block.Substring(ifBlockStart, ifBlockEnd - ifBlockStart);

                    // Recursively parse the code within the `if` block
                    ParseIfElseClausesRecursively(ifBlockContent, clauses, 0);
                }

                // If there's an `else`, extract the code within the `else` block
                if (elsePosition != -1)
                {
                    int elseBlockStart = block.IndexOf('{', ifBlockEnd) + 1;
                    int elseBlockEnd = block.LastIndexOf('}');

                    if (elseBlockStart > 0 && elseBlockEnd > elseBlockStart) // Ensure valid indices
                    {
                        string elseBlockContent = block.Substring(elseBlockStart, elseBlockEnd - elseBlockStart);

                        // Recursively parse the code within the `else` block
                        ParseIfElseClausesRecursively(elseBlockContent, clauses, 0);
                    }
                }

                // Move the startIndex to just after the processed block
                var blockEndIndex = ifIndex + block.Length;
                if (blockEndIndex >= code.Length)
                    break;

                startIndex = blockEndIndex;
            }
        }


        public class IfElseClause
        {
            public int IfIndex { get; set; }
            public string Block { get; set; }
            public int ElseIndex { get; set; } // Index of the outermost `else`
        }
    }
}
