using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Interpreter
{
    internal static class IfElseObjExp
    {
        private static Dictionary<int, IfElseObj> Master = new Dictionary<int, IfElseObj>();

        internal static void ProcessInput(string input, bool isElse, int parentId, int layer, int position)
        {
            var obj = new IfElseObj
            {
                Input = input,
                Else = isElse,
                ParentId = parentId,
                Id = Master.Count,
                Layer = layer + 1,
                Position = position
            };

            var check = IrtKernel.ContainsKeywordWithOpenParenthesis(input, "if");
            if (!check)
            {
                obj.Nested = false;
                obj.Commands = IrtKernel.GetBlocks(input);

				Master.Add(obj.Id, obj);
                return;
            }

            obj.Nested = true;

            obj.Commands = IrtKernel.GetBlocks(input);
			Master.Add(obj.Id, obj);
        }
	}
}