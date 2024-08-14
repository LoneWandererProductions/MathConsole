using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Interpreter
{
    internal static class IfElseObjExp
    {
        private static Dictionary<int, IfElseObj> Master = new Dictionary<int, IfElseObj>();

        private static void ProcessInput(string input, bool isElse, int parentId, int layer, int position)
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

            var ifIndex = IrtKernel.FindFirstKeywordIndex(input, "if");
            if (ifIndex == -1)
            {
                obj.Nested = false;
                obj.Commands = new Dictionary<int, (string, string)>
                {
                    {0, ("Command", input)}
                };
                Master.Add(obj.Id, obj);
                return;
            }

            obj.Nested = true;
            var formattedBlocks = new Dictionary<int, (string, string)>();
            var keepParsing = true;

            while (keepParsing)
            {
                ifIndex = IrtKernel.FindFirstKeywordIndex(input, "if");

                if (ifIndex == -1)
                {
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        formattedBlocks.Add(formattedBlocks.Count, ("Command", input));
                    }

                    keepParsing = false;
                }
                else
                {
                    //tODO make a switch for if and else
                    var beforeIf = input.Substring(0, ifIndex).Trim();
                    if (!string.IsNullOrWhiteSpace(beforeIf))
                    {
                        formattedBlocks.Add(formattedBlocks.Count, ("Command", beforeIf));
                    }

                    input = input.Substring(ifIndex);
                    var (ifElseBlock, elsePosition) = IrtKernel.ExtractFirstIfElse(input);

                    // Add the current if block
                    formattedBlocks.Add(formattedBlocks.Count, ("If", beforeIf));

                    // Process the nested blocks recursively
                    ProcessInput(ifElseBlock, false, obj.Id, obj.Layer + 1, obj.Position);

                    // Remove the processed block from the input string
                    input = input.Substring(ifElseBlock.Length).Trim();
                }
            }

            obj.Commands = formattedBlocks;
            Master.Add(obj.Id, obj);
        }
    }
}