using System;
using System.Threading;
using Interpreter;

namespace MathConsole
{
    internal static class Program
    {
        private static Prompt _prompt;

        private static readonly object ConsoleLock = new object();
        private static bool _isEventTriggered;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        private static void Main()
        {
            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(Namespaces.Statistics, "Statistics", Namespaces.ExtensionStatistics);
            _prompt.AddCommands(Namespaces.Matrix, "Matrix");

            _prompt.Callback("Hello World!");

            while (true)
            {
                lock (ConsoleLock)
                {
                    if (!_isEventTriggered)
                    {
                        _prompt.Callback("Enter something: ");
                        var input = Console.ReadLine();

                        _prompt.StartConsole(input);
                    }
                    else
                    {
                        _prompt.Callback("Event is processing. Please wait...");
                    }
                }

                Thread.Sleep(500); // Small delay to prevent tight loop
            }
        }

        /// <summary>
        ///     Listen to Messages
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Type</param>
        private static void SendLogs(object sender, string e)
        {
            lock (ConsoleLock)
            {
                _isEventTriggered = true;
                Console.WriteLine(e);

                _isEventTriggered = false;
            }
        }

        /// <summary>
        ///     Listen to Commands
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Type</param>
        private static void SendCommands(object sender, OutCommand e)
        {
            lock (ConsoleLock)
            {
                _isEventTriggered = true;

                // Simulate event processing
                _prompt.Callback("\nEvent triggered. Processing...");

                HandleCommands(e);

                if(e.ExtensionUsed) HandleExtensionCommands(e);

                _prompt.Callback("Event processing completed.");

                _isEventTriggered = false;
            }
        }

        /// <summary>
        /// Handles the commands.
        /// </summary>
        /// <param name="outCommand">The out command.</param>
        private static void HandleCommands(OutCommand outCommand)
        {
            //TODO add switch for Namespace

            switch (outCommand.Command)
            {
                //Just show some stuff
                case -1:
                    _prompt.Callback(outCommand.ErrorMessage);
                    break;

                case 0:
                    //TODO
                    break;
                //close the window
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
                case 99:
                    // Simulate some work
                    _prompt.Callback("The application will close after a short delay.");

                    _prompt.Dispose();
                    // Introduce a small delay before closing
                    Thread.Sleep(3000); // Delay for 3000 milliseconds (3 seconds)
                    // Close the console application
                    Environment.Exit(0);
                    break;

                default:
                    //TODO
                    _prompt.CallbacksWindow("");
                    break;
            }
        }

        private static void HandleExtensionCommands(OutCommand outCommand)
        {
            //TODO
        }
    }
}
