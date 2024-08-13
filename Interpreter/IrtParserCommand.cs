using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtendedSystemObjects;

namespace Interpreter
{
    internal static class IrtParserCommand
    {
        private static int _idCounter;

        public static string GenerateFormattedOutput(List<(string Category, string Clause, string ParentCategory)> categorizedClauses)
        {
            var output = new StringBuilder();
            var openBlocks = new Stack<string>();
            var clauseDict = categorizedClauses.ToDictionary(clause => clause.Category);

            void ProcessClause((string Category, string Clause, string ParentCategory) clause)
            {
                var categoryParts = clause.Category?.Split('_');
                var layer = categoryParts?.Length > 2 ? categoryParts[2] : "unknown";

                // Process the 'if' block
                if (clause.Category.StartsWith("if"))
                {
                    output.AppendLine($"If_layer_{layer}_begin: {clause.Clause}");
                    output.AppendLine("// Content of If block goes here");
                    openBlocks.Push(clause.Category);

                    // Process nested clauses
                    var nestedClauses = categorizedClauses.Where(c => c.ParentCategory == clause.Category).ToList();
                    foreach (var nestedClause in nestedClauses)
                    {
                        ProcessClause(nestedClause);
                    }

                    output.AppendLine($"If_layer_{layer}_end");
                    openBlocks.Pop();
                }
                // Process the 'else' block
                else if (clause.Category.StartsWith("else"))
                {
                    output.AppendLine($"Else_layer_{layer}_begin: {clause.Clause}");
                    output.AppendLine("// Content of Else block goes here");
                    openBlocks.Push(clause.Category);

                    // Process nested clauses
                    var nestedClauses = categorizedClauses.Where(c => c.ParentCategory == clause.Category).ToList();
                    foreach (var nestedClause in nestedClauses)
                    {
                        ProcessClause(nestedClause);
                    }

                    output.AppendLine($"Else_layer_{layer}_end");
                    openBlocks.Pop();
                }
            }

            // Process only root elements (those without a parent)
            var rootClauses = categorizedClauses.Where(c => string.IsNullOrEmpty(c.ParentCategory)).ToList();
            foreach (var rootClause in rootClauses)
            {
                ProcessClause(rootClause);
            }

            return output.ToString();
        }



        /// <summary>
        /// Categorizes if else clauses.
        /// </summary>
        /// <param name="clauses">The clauses.</param>
        /// <returns>Converted List of parameters we will use to create a parameter list</returns>
        public static List<(string Category, string Clause, string ParentCategory)> CategorizeIfElseClauses(List<IfElseClause> clauses)
        {
            var tupleList = new List<(string Category, string Clause, string ParentCategory)>();
            var idToCategory = new Dictionary<string, string>();

            // Process the outermost layer (Layer 0)
            foreach (var clause in clauses.Where(c => c.Layer == 0).ToList())
            {
                var categoryIf = $"if_layer_{clause.Id}";
                var categoryElse = $"else_layer_{clause.Id}";

                idToCategory[clause.Id] = categoryIf;

                tupleList.Add((categoryIf, clause.IfClause, null));
                tupleList.Add((categoryElse, clause.ElseClause, null));
            }

            // Process deeper layers (Layer > 0)
            foreach (var layer in clauses.Where(c => c.Layer > 0).OrderBy(c => c.Layer))
            {
                var parent = clauses.FirstOrDefault(c => c.Layer == layer.Layer - 1);
                var parentCategory = idToCategory.ContainsKey(parent.Id) ? idToCategory[parent.Id] : null;

                var categoryIf = $"if_layer_{layer.Id}";
                var categoryElse = $"else_layer_{layer.Id}";

                // Add the deeper layer clauses to the tuple list
                tupleList.Add((categoryIf, layer.IfClause, parentCategory));
                tupleList.Add((categoryElse, layer.ElseClause, parentCategory));

                // Update category mapping
                idToCategory[layer.Id] = categoryIf;
            }

            return tupleList;
        }

        /// <summary>
        /// Parses the given code string to extract all If-Else clauses.
        /// </summary>
        /// <param name="code">The code string containing If-Else clauses.</param>
        /// <returns>A list of IfElseClause objects representing each If-Else clause found.</returns>
        public static List<IfElseClause> ParseIfElseClauses(string code)
        {
            var clauses = new List<IfElseClause>();
            ParseIfElseClausesRecursively(code, clauses, 0);
            return clauses;
        }

        /// <summary>
        /// Recursively parses If-Else clauses from the code string.
        /// </summary>
        /// <param name="code">The code string containing If-Else clauses.</param>
        /// <param name="clauses">The list to which found If-Else clauses are added.</param>
        /// <param name="layer">The current nesting layer of the If-Else clauses.</param>
        private static void ParseIfElseClausesRecursively(string code, ICollection<IfElseClause> clauses, int layer)
        {
            while (true)
            {
                var ifIndex = IrtKernel.FindFirstIfIndex(code, "if");
                if (ifIndex == -1)
                    break;

                var codeFromIfIndex = code.Substring(ifIndex);
                var (block, elsePosition) = IrtKernel.ExtractFirstIfElse(codeFromIfIndex);

                if (string.IsNullOrWhiteSpace(block)) break;

                var ifElseClause = CreateIfElseClause(code, block, elsePosition, layer);
                clauses.Add(ifElseClause);

                var innerIfClause = ExtractInnerIfElse(ifElseClause.IfClause);
                if (innerIfClause.Length > 0)
                    ParseIfElseClausesRecursively(innerIfClause, clauses, layer + 1);

                var innerElseClause = ExtractInnerIfElse(ifElseClause.ElseClause);
                if (innerElseClause.Length > 0)
                    ParseIfElseClausesRecursively(innerElseClause, clauses, layer + 1);

                break;
            }
        }

        /// <summary>
        /// Creates an IfElseClause object from the extracted If-Else block.
        /// </summary>
        /// <param name="code">The original code string.</param>
        /// <param name="block">The extracted block containing If-Else code.</param>
        /// <param name="elsePosition">The position of the 'else' keyword in the block.</param>
        /// <param name="layer">The nesting layer of the If-Else clause.</param>
        /// <returns>An IfElseClause object representing the parsed If-Else clause.</returns>
        private static IfElseClause CreateIfElseClause(string code, string block, int elsePosition, int layer)
        {
            var ifClause = block.Substring(0, elsePosition).Trim();
            var elseClause = elsePosition == -1 ? null : block.Substring(elsePosition).Trim();
            // Generate a unique ID for each clause
            var uniqueId = GenerateUniqueId();

            return new IfElseClause
            {
                Id = uniqueId,
                Parent = code,
                IfClause = ifClause,
                ElseClause = elseClause,
                Layer = layer
            };
        }

        /// <summary>
        /// Builds a categorized dictionary of commands from the given input string.
        /// </summary>
        /// <param name="input">The input string containing command definitions.</param>
        /// <returns>A CategorizedDictionary containing the parsed commands.</returns>
        internal static CategorizedDictionary<int, string> BuildCommand(string input)
        {
            // Remove unnecessary characters from the input string
            input = IrtKernel.RemoveLastOccurrence(input, IrtConst.AdvancedClose);
            input = IrtKernel.RemoveFirstOccurrence(input, IrtConst.AdvancedOpen);
            input = input.Trim();

            var formattedBlocks = new List<string>();
            var keepParsing = true;

            while (keepParsing)
            {
                var ifIndex = IrtKernel.FindFirstIfIndex(input, "if");
                if (ifIndex == -1)
                {
                    keepParsing = false;
                    if (!string.IsNullOrWhiteSpace(input))
                        formattedBlocks.Add(input.Trim());
                }
                else
                {
                    var beforeIf = input.Substring(0, ifIndex).Trim();
                    if (!string.IsNullOrWhiteSpace(beforeIf)) formattedBlocks.Add(beforeIf);

                    input = input.Substring(ifIndex);
                    var (ifElseBlock, elsePosition) = IrtKernel.ExtractFirstIfElse(input);

                    if (elsePosition == -1)
                    {
                        formattedBlocks.Add(ifElseBlock.Trim());
                    }
                    else
                    {
                        var ifBlock = input.Substring(0, elsePosition).Trim();
                        var elseBlock = input.Substring(elsePosition, ifElseBlock.Length - elsePosition).Trim();
                        formattedBlocks.Add(ifBlock);
                        formattedBlocks.Add(elseBlock);
                    }

                    input = input.Substring(ifElseBlock.Length).Trim();
                }
            }

            var commandRegister = new CategorizedDictionary<int, string>();
            var commandIndex = 0;

            foreach (var block in formattedBlocks)
            {
                var keywordIndex = IrtKernel.GetCommandIndex(block, IrtConst.InternContainerCommands);

                switch (keywordIndex)
                {
                    case 0:
                    case 1:
                        commandRegister.Add(IrtConst.InternContainerCommands[keywordIndex].Command, commandIndex, block);
                        break;

                    default:
                        foreach (var trimmedSubCommand in IrtKernel.SplitParameter(block, IrtConst.NewCommand)
                            .Select(subCommand => subCommand.Trim()))
                        {
                            keywordIndex = IrtKernel.GetCommandIndex(trimmedSubCommand, IrtConst.InternContainerCommands);
                            commandIndex++;

                            switch (keywordIndex)
                            {
                                case 2:
                                case 3:
                                    commandRegister.Add(IrtConst.InternContainerCommands[keywordIndex].Command, commandIndex, trimmedSubCommand);
                                    break;

                                default:
                                    if (!string.IsNullOrEmpty(trimmedSubCommand))
                                        commandRegister.Add("COMMAND", commandIndex, trimmedSubCommand);
                                    break;
                            }
                        }

                        break;
                }

                commandIndex++;
            }

            return commandRegister;
        }

        /// <summary>
        /// Removes the outermost If-Else structure from the given code string.
        /// </summary>
        /// <param name="code">The code string containing an If-Else structure.</param>
        /// <returns>The code string with the outer If-Else structure removed.</returns>
        private static string ExtractInnerIfElse(string code)
        {
            if (!code.StartsWith("if", StringComparison.OrdinalIgnoreCase)) return code;

            var openBraceIndex = code.IndexOf('{');
            var closeBraceIndex = FindBlockEnd(code, openBraceIndex);

            return closeBraceIndex != -1
                ? code.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1).Trim()
                : code;
        }

        /// <summary>
        /// Finds the index of the closing brace that matches the opening brace starting at the given index.
        /// </summary>
        /// <param name="code">The code string.</param>
        /// <param name="start">The index of the opening brace.</param>
        /// <returns>The index of the matching closing brace, or -1 if not found.</returns>
        private static int FindBlockEnd(string code, int start)
        {
            var braceCount = 0;

            for (var i = start; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case '{':
                        braceCount++;
                        break;
                    case '}':
                        braceCount--;
                        if (braceCount == 0) return i;

                        break;
                }
            }

            return -1;
        }

        /// <summary>
        /// Generates the unique identifier.
        /// </summary>
        /// <returns>Count ups</returns>
        private static string GenerateUniqueId()
        {
            // Use a static counter to generate unique IDs
            return (++_idCounter).ToString();
        }
    }
}
