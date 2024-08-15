using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Interpreter
{
    internal static class IfElseObjExp
    {
        /// <summary>
        /// Parses the given code string to extract all If-Else clauses.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>A list of IfElseClause objects representing each If-Else clause found.</returns>
        internal static Dictionary<int, IfElseObj> ParseIfElseClauses(string input)
        {
            var master = new Dictionary<int, IfElseObj>();
            ProcessInput(input, false, -1, -1, 0, master);
            return master;
        }

        internal static void ProcessInput(string input, bool isElse, int parentId, int layer, int position,
            Dictionary<int, IfElseObj> master)
        {
            var obj = new IfElseObj
            {
                Input = input,
                Else = isElse,
                ParentId = parentId,
                Id = master.Count,
                Layer = layer + 1,
                Position = position
            };

            var check = IrtKernel.ContainsKeywordWithOpenParenthesis(input, "if");
            if (!check)
            {
                obj.Nested = false;
                obj.Commands = IrtKernel.GetBlocks(input);

                master.Add(obj.Id, obj);
                return;
            }

            obj.Nested = true;

            obj.Commands = IrtKernel.GetBlocks(input);
            master.Add(obj.Id, obj);
        }
    }
}