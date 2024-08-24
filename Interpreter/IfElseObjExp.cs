﻿using System;
using System.Collections.Generic;

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

            obj.Commands = IrtKernel.GetBlocks(input);


            master.Add(obj.Id, obj);

            foreach (var command in obj.Commands)
            {
                //we should either remove the first if or else or ignore the first else
                check = IrtKernel.ContainsKeywordWithOpenParenthesis(command.Value, "if");
                if (check) continue;

                var category = command.Category;

                var isElseBlock = category.Equals("Else", StringComparison.OrdinalIgnoreCase);
                ProcessInput(command.Value, isElseBlock, obj.Id, obj.Layer, command.Key, master);
            }
        }
    }
}