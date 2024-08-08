﻿using System;
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
                int ifIndex = IrtIfElseParser.FindFirstIfIndex(code.Substring(startIndex));
                if (ifIndex == -1)
                    break;

                // Adjust ifIndex to the original code's index
                ifIndex += startIndex;

                // Extract the if-else block starting from `ifIndex`
                string codeFromIfIndex = code.Substring(ifIndex);
                var (block, elsePosition) = IrtIfElseParser.ExtractFirstIfElse(codeFromIfIndex);

                // Calculate the correct index for `elsePosition`
                int actualElseIndex = elsePosition == -1 ? -1 : elsePosition + ifIndex;

                // Save the extracted block and its positions
                clauses.Add(new IfElseClause
                {
                    IfIndex = ifIndex,
                    Block = block,
                    ElseIndex = actualElseIndex
                });

                // Move the startIndex to just after the processed block
                int blockEndIndex = ifIndex + block.Length;
                string remainingCode = code.Substring(blockEndIndex);

                // Continue parsing the remaining code from the beginning of the remaining string
                // Note: startIndex is reset to 0 here to process from the beginning of the remaining string
                startIndex = 0;
                code = remainingCode;
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
