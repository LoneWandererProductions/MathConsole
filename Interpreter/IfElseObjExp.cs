using System;
using System.Collections.Generic;
using ExtendedSystemObjects;

namespace Interpreter
{
    public static class IfElseObjExp
    {
        /// <summary>
        ///     Parses the given code string to extract all If-Else clauses.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>A list of IfElseClause objects representing each If-Else clause found.</returns>
        public static Dictionary<int, IfElseObj> ParseIfElseClauses(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            var master = new Dictionary<int, IfElseObj>();
            ProcessInput(input, false, -1, -1, 0, master);
            return master;
        }

        /// <summary>
        /// Processes the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="isElse">if set to <c>true</c> [is else].</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="position">The position.</param>
        /// <param name="master">The master.</param>
        public static void ProcessInput(string input, bool isElse, int parentId, int layer, int position,
            IDictionary<int, IfElseObj> master)
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

            var commands = IrtKernel.GetBlocks(input);
            obj.Commands ??= new CategorizedDictionary<int, string>();  // Initialize Commands if null

            master.Add(obj.Id, obj);

            foreach (var (key, category, value) in commands)
            {
                // We should either remove the first if or else or ignore the first else
                check = IrtKernel.ContainsKeywordWithOpenParenthesis(value, "if");
                if (!check)
                {
                    obj.Commands.Add(category, key, value);
                    continue;
                }

                var isElseBlock = category.Equals("Else", StringComparison.OrdinalIgnoreCase);
                ProcessInput(value, isElseBlock, obj.Id, obj.Layer, key, master);
            }
        }
    }
}