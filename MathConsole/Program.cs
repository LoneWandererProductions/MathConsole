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
            Console.WriteLine("Hello World!");

            _prompt = new Prompt();
            _prompt.SendLogs += SendLogs;
            _prompt.SendCommands += SendCommands;
            _prompt.Initiate(Namespaces.Statistics, "Statistics", Namespaces.ExtensionStatistics);

            while (true)
            {
                lock (ConsoleLock)
                {
                    if (!_isEventTriggered)
                    {
                        Console.WriteLine("Enter something: ");
                        var input = Console.ReadLine();
                        Console.WriteLine("You entered: " + input);
                        _prompt.StartConsole(input);
                    }
                    else
                    {
                        Console.WriteLine("Event is processing. Please wait...");
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
            Console.WriteLine(e);
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
                Console.WriteLine("\nEvent triggered. Processing...");

                HandleCommands(e);

                if(e.ExtensionUsed) HandleExtensionCommands(e);

                Console.WriteLine("Event processing completed.");

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
                    //TODO
                    Console.WriteLine("Caused an error...");
                    break;

                case 0:
                    //TODO
                    _prompt.Callbacks("");
                    break;
                //close the window
                case 1:
                    //TODO
                    _prompt.Callbacks("");
                    break;

                case 2:
                    //TODO
                    _prompt.Callbacks("");
                    break;

                case 3:
                    //TODO
                    _prompt.Callbacks("");
                    break;

                case 4:
                    //TODO
                    _prompt.Callbacks("");
                    break;
                case 99:
                    // Simulate some work
                    Console.WriteLine("The application will close after a short delay.");

                    _prompt.Dispose();
                    // Introduce a small delay before closing
                    Thread.Sleep(3000); // Delay for 3000 milliseconds (3 seconds)
                    // Close the console application
                    Environment.Exit(0);
                    break;

                default:
                    //TODO
                    _prompt.Callbacks("");
                    break;
            }
        }

        private static void HandleExtensionCommands(OutCommand outCommand)
        {
            //TODO
        }
    }
}
