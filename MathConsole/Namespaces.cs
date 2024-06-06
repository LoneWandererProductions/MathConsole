using System.Collections.Generic;
using Interpreter;

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

        /// <summary>
        ///     Our Command Input Register
        /// </summary>
        internal static readonly Dictionary<int, InCommand> Matrix = new()
        {
            {
                0,
                new InCommand
                {
                    Command = "Matrix",
                    Description =
                        "Load a matrix into the internal memory. First is id of the matrix, if id is equal, existing matrix will be overwriten, second, third, height, width, the rest matrix data as double. Min 7 Parameter",
                    ParameterCount = -7
                }
            },
            {
                1,
                new InCommand
                {
                    Command = "ListMatrix",
                    Description = "Show all saved Matrizes with value.",
                    ParameterCount = 0
                }
            },
            {
                2,
                new InCommand
                {
                    Command = "SolveMatrix",
                    Description = "Calculate some stuff for the Matrix with the Id as parameter.",
                    ParameterCount = 1
                }
            },
            {
                3,
                new InCommand
                {
                    Command = "Mulitply",
                    Description = "Mulitply the Matrizes defined by id, min Parameter count is 2.",
                    ParameterCount = -2
                }
            },
            {
                4,
                new InCommand
                {
                    Command = "Add",
                    Description = "Mulitply the Matrizes defined by id, min Parameter count is 2.",
                    ParameterCount = -2
                }
            },
            {
                5,
                new InCommand
                {
                    Command = "Substract",
                    Description = "Mulitply the Matrizes defined by id, min Parameter count is 2.",
                    ParameterCount = -2
                }
            }
        };
    }
}