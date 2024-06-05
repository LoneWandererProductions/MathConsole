using ExtendedSystemObjects;
using Interpreter;
using Mathematics;
using System;
using System.Collections.Generic;

namespace MatrixPlugin
{
    public static class MatrixHandler
    {
        private static Dictionary<int, BaseMatrix> Matrix = new Dictionary<int, BaseMatrix>();

        public static string HandleCommands(OutCommand outCommand)
        {
            //TODO add switch for Namespace

            switch (outCommand.Command)
            {
                case 0:
                    return SetMatrix(outCommand.Parameter);
                case 1:
                    //TODO
                    break;

                case 2:
                    //TODO
                    break;

                case 3:
                    //TODO
                    break;

                case 4:
                    //TODO
                    break;

                default:
                    return string.Empty;
            }

            return string.Empty;
        }


        public static string SetMatrix(List<string> parameter)
        {
            var collection = new List<int>();
            bool check;

            foreach (string param in parameter)
            {
                check = int.TryParse(param, out int number);

                if (!check) return "Matrix could not be added, parameter was not an int value.";

                collection.Add(number);
            }

            if (collection.Count % 2 == 0) return "Matrix could not be added, wrong number of parameters.";

            var id = collection[0];
            var height = collection[1];
            var width = collection[2];

            if(height * width < (collection.Count -3)) return "Matrix could not be added, not enough valued provided to fill up the matrix.";

            var matrix = new BaseMatrix(height, width);

            var count = -1;

            for (int i = 3; i < collection.Count; i++)
            {
                var element = collection[i];
                count++;

                var x = count % width;
                var y = count / width;

                matrix.Matrix[x, y] = element;
            }

            //overwrite existing matrix
            Matrix.AddDistinct(id, matrix);

            return "Matrix added at Position:" + id;
        }
    }
}
