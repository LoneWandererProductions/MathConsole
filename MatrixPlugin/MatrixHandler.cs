using Interpreter;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MatrixPlugin
{
    public static class MatrixHandler
    {
        private static bool HandleCommands(OutCommand outCommand)
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
                    return false;
            }

            return false;
        }


        public static bool SetMatrix(List<string> parameter)
        {
            return false;
        }
    }
}
