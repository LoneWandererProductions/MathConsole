/*
 * TODO: Implement loop handling with translation to if-else:
 * 
 * 1. Translate while(condition) {...} to if(condition) {...} else {repeat}:
 *    - Use a custom "repeat" command to return to the if clause.
 *    - Simplify flow control by avoiding complex loop handling.
 * 
 * 2. Implement "repeat" as a generic command:
 *    - When "repeat" is hit, it returns to the calling flow statement.
 *    - Ensure correct flow by tracking execution within the workflow.
 * 
 * 3. Manage the stack:
 *    - Translate input code into an execution workflow.
 *    - Store all translated commands in a dictionary for execution.
 * 
 * 4. Future expansion:
 *    - Add support for "continue" and "break" commands.
 *    - Follow the same approach as "repeat" to manage these in the workflow.
 */


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
        /// <param name="input">The input code string</param>
        /// <returns>A dictionary of IfElseObj objects representing each If-Else clause found.</returns>
        public static Dictionary<int, IfElseObj> ParseIfElseClauses(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            var ifElseClauses = new Dictionary<int, IfElseObj>();
            ProcessInput(input, isElse: false, parentId: -1, layer: -1, position: 0, ifElseClauses);
            return ifElseClauses;
        }

        /// <summary>
        /// Processes the input and splits the input into IfElseObj objects.
        /// </summary>
        private static void ProcessInput(string input, bool isElse, int parentId, int layer, int position,
            IDictionary<int, IfElseObj> ifElseClauses)
        {
            var obj = CreateIfElseObj(input, isElse, parentId, layer, position);
            ifElseClauses.Add(obj.Id, obj);

            var commands = IrtKernel.GetBlocks(input);
            obj.Commands ??= new CategorizedDictionary<int, string>();

            // Process each block of commands
            foreach (var (key, category, value) in commands)
            {
                var containsIf = IrtKernel.ContainsKeywordWithOpenParenthesis(value, "if");
                if (!containsIf)
                {
                    obj.Commands.Add(category, key, value); // Add the block if it doesn't contain 'if'
                    continue;
                }

                // Recursively process if we find a new 'if' block
                var isElseBlock = category.Equals("Else", StringComparison.OrdinalIgnoreCase);
                ProcessInput(value, isElseBlock, obj.Id, obj.Layer, key, ifElseClauses);
            }
        }

        /// <summary>
        /// Helper method to create an IfElseObj instance.
        /// </summary>
        private static IfElseObj CreateIfElseObj(string input, bool isElse, int parentId, int layer, int position)
        {
            return new IfElseObj
            {
                Input = input,
                Else = isElse,
                ParentId = parentId,
                Id = Guid.NewGuid().GetHashCode(), // Replace with master.Count for simple sequential id
                Layer = layer + 1,
                Position = position
            };
        }
    }
}
