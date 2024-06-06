using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Interpreter;
using Mathematics;

namespace MatrixPlugin
{
    public static class MatrixHandler
    {
        private static readonly Dictionary<int, BaseMatrix> Matrix = new();

        public static string HandleCommands(OutCommand outCommand)
        {
            switch (outCommand.Command)
            {
                case 0:
                    // Set matrix
                    return SetMatrix(outCommand.Parameter);

                case 1:
                    // List matrix
                    return ListMatrix();

                case 2:
                    // Solve matrix
                    return SolveMatrix(outCommand.Parameter);

                case 3:
                    // Multiply
                    return MultiplyMatrices(outCommand.Parameter);

                case 4:
                    // Sum
                    return SumMatrices(outCommand.Parameter);

                case 5:
                    // Substract
                    return SubstractMatrices(outCommand.Parameter);

                default:
                    return "Invalid command.";
            }
        }

        private static string SetMatrix(List<string> parameter)
        {
            if (!int.TryParse(parameter[0], out var id)) return "Invalid parameter: id";
            if (!int.TryParse(parameter[1], out var height)) return "Invalid parameter: height";
            if (!int.TryParse(parameter[2], out var width)) return "Invalid parameter: width";

            if (height * width != parameter.Count - 3) return "Mismatch between matrix size and provided values.";

            var matrix = new BaseMatrix(height, width);
            var count = 0;

            for (var i = 3; i < parameter.Count; i++)
            {
                if (!double.TryParse(parameter[i], out var number)) return $"Invalid number: {parameter[i]}";

                var x = count % width;
                var y = count / width;

                matrix.Matrix[x, y] = number;
                count++;
            }

            Matrix[id] = matrix;
            return $"Matrix added at Position: {id}";
        }

        private static string ListMatrix()
        {
            return Matrix.Aggregate(string.Empty, (current, matrix) =>
                $"{current}Id: {matrix.Key} : {matrix.Value}{Environment.NewLine}");
        }

        private static string SolveMatrix(IEnumerable<string> outCommandParameter)
        {
            var id = outCommandParameter.First();
            if (!int.TryParse(id, out var number)) return $"Invalid parameter: {id}";

            if (!Matrix.ContainsKey(number)) return "Requested Matrix does not exist.";

            var matrix = Matrix[number];
            var result = $"Inverse Matrix: {Environment.NewLine}";

            try
            {
                result += matrix.Inverse().ToString();
            }
            catch (NotImplementedException ex)
            {
                Trace.WriteLine(ex);
                return "Matrix inversion not implemented for non Cubic Matrices.";
            }

            try
            {
                var (l, u) = matrix.LuDecomposition();
                result += $"{Environment.NewLine}L Matrix: {l}{Environment.NewLine}U Matrix: {u}";
            }
            catch (NotImplementedException ex)
            {
                Trace.WriteLine(ex);
                return "LU decomposition not implemented for non Cubic Matrices.";
            }

            try
            {
                var determinant = matrix.Determinant();
                result += $"{Environment.NewLine}Determinant: {determinant}";
            }
            catch (ArithmeticException ex)
            {
                Trace.WriteLine(ex);
                return "Matrix determinant calculation failed.";
            }

            return result;
        }

        private static string MultiplyMatrices(List<string> parameter)
        {
            BaseMatrix matrix = null;

            for (var i = 0; i < parameter.Count; i++)
            {
                if (!int.TryParse(parameter[i], out var number)) return $"Invalid number: {parameter[i]}";

                try
                {
                    if (i == 0) matrix = Matrix[number];
                    else matrix *= Matrix[number];
                }
                catch (ArithmeticException ex)
                {
                    return $"Error multiplying matrices: {ex.Message}";
                }
            }

            return matrix?.ToString() ?? "No matrices provided for multiplication.";
        }

        private static string SumMatrices(List<string> parameter)
        {
            return PerformOperation(parameter, (a, b) => a + b, "sum");
        }

        private static string SubstractMatrices(List<string> parameter)
        {
            return PerformOperation(parameter, (a, b) => a - b, "subtraction");
        }

        private static string PerformOperation(List<string> parameter,
            Func<BaseMatrix, BaseMatrix, BaseMatrix> operation, string operationName)
        {
            BaseMatrix matrix = null;

            for (var i = 0; i < parameter.Count; i++)
            {
                if (!int.TryParse(parameter[i], out var number)) return $"Invalid number: {parameter[i]}";

                try
                {
                    if (i == 0) matrix = Matrix[number];
                    else matrix = operation(matrix, Matrix[number]);
                }
                catch (ArithmeticException ex)
                {
                    return $"Error performing {operationName} operation: {ex.Message}";
                }
            }

            return matrix?.ToString() ?? $"No matrices provided for {operationName}.";
        }
    }
}