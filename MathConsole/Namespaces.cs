using Interpreter;
using System.Collections.Generic;

namespace MathConsole
{
    internal static class Namespaces
    {
        /// <summary>
        ///     Our Command Input Register
        /// </summary>
        internal static readonly Dictionary<int, InCommand> Statistics = new()
        {
            {
                0,
                new InCommand
                {
                    Command = "Load",
                    Description = "Load a csv file, parameter is the path.",
                    ParameterCount = 1
                }
            },
            {
                99,
                new InCommand
                {
                    Command = "Close",
                    Description = "Close the prompt",
                    ParameterCount = 0
                }
            },
            {
                1,
                new InCommand
                {
                    Command = "Calculate",
                    Description = "Calculate the statistic data for the loaded file.",
                    ParameterCount = 0
                }
            },
            {
                2,
                new InCommand
                {
                    Command = "Calculate",
                    Description = "Calculate the statistic data for the loaded file, the parameter defines the column.",
                    ParameterCount = 1
                }
            }
        };

        /// <summary>
        ///     Our Command Input Register
        /// </summary>
        internal static readonly Dictionary<int, InCommand> ExtensionStatistics = new()
        {
            {
                0,
                new InCommand
                {
                    Command = "Save",
                    Description = "Save the results to path, with predefined name.",
                    ParameterCount = 0
                }
            },
            {
                1,
                new InCommand
                {
                    Command = "Save",
                    Description = "Save the results to path, with a user defined name.",
                    ParameterCount = 1
                }
            }
        };
    }
}