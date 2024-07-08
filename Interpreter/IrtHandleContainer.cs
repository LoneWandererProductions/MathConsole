/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtHandleContainer.cs
 * PURPOSE:     Handles all the stuff with container
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Local

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Interpreter
{
    /// <inheritdoc />
    /// <summary>
    ///     Handle our Container
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal sealed class IrtHandleContainer : IDisposable
    {
        /// <summary>
        ///     The disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     The irt handle internal
        /// </summary>
        private IrtHandleInternal _irtHandleInternal;

        /// <summary>
        ///     The prompt
        /// </summary>
        private Prompt _prompt;

        /// <summary>
        ///     Prevents a default instance of the <see cref="IrtHandleContainer" /> class from being created.
        /// </summary>
        private IrtHandleContainer()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IrtHandleContainer" /> class.
        /// </summary>
        /// <param name="irtHandleInternal">The irt handle internal.</param>
        /// <param name="prompt">Call back to the main entry</param>
        internal IrtHandleContainer(IrtHandleInternal irtHandleInternal, Prompt prompt)
        {
            _irtHandleInternal = irtHandleInternal;
            _prompt = prompt;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Releases all resources used by the <see cref="T:Interpreter.IrtHandleContainer" /> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Processes the container.
        /// </summary>
        /// <param name="parameterPart">Parameter Part.</param>
        internal void CommandContainer(string parameterPart)
        {
            parameterPart = Irt.RemoveLastOccurrence(parameterPart, IrtConst.AdvancedClose);
            parameterPart = Irt.RemoveFirstOccurrence(parameterPart, IrtConst.AdvancedOpen);

            GenerateCommands(parameterPart);
        }

        /// <summary>
        ///     Commands the batch execute.
        /// </summary>
        /// <param name="parameterPart">The parameter part.</param>
        internal void CommandBatchExecute(string parameterPart)
        {
            parameterPart = Irt.RemoveParenthesis(parameterPart, IrtConst.BaseOpen, IrtConst.BaseClose);
            parameterPart = IrtHelper.ReadBatchFile(parameterPart);

            if (string.IsNullOrEmpty(parameterPart))
            {
                _irtHandleInternal.SetError(IrtConst.ErrorFileNotFound);
                return;
            }

            GenerateCommands(parameterPart);
        }

        /// <summary>
        ///     Generates the commands.
        ///     Here we generate our list
        /// </summary>
        /// <param name="parameterPart">The parameter part.</param>
        private void GenerateCommands(string parameterPart)
        {
            var commands = Irt.SplitParameter(parameterPart, IrtConst.NewCommand).ToList();
            var currentPosition = 0;

            while (currentPosition < commands.Count)
            {
                var com = commands[currentPosition];

                // Check if it contains a Keyword
                var key = Irt.CheckForKeyWord(com, IrtConst.InternContainerCommands);

                if (key == IrtConst.Error)
                {
                    // Just because we run a container or a batch, we still have to log it
                    _prompt.AddToLog(com);
                    // Mostly use the prompt to add a layer of security
                    _prompt.ConsoleInput(com);
                }
                else
                {
                    switch (key)
                    {
                        // if
                        case 0:
                            currentPosition = HandleIfElseBlock(commands, currentPosition);
                            break;
                        // else (ignored, handled by if)
                        case 1:
                            break;
                        // goto
                        case 2:
                            // If it is a jump command, change the current position
                            currentPosition = IsJumpCommand(com, key, out var jumpPosition, commands)
                                ? Math.Clamp(jumpPosition, 0, commands.Count - 1)
                                : IrtConst.Error;
                            break;
                        // label
                        case 3:
                            Trace.WriteLine(com);
                            break;
                    }
                }

                // Check for error condition to break the loop
                if (currentPosition == IrtConst.Error)
                {
                    var message = Logging.SetLastError($"{IrtConst.JumpLabelNotFoundError}{com}", 0);
                    _prompt.SendLogs(this, message);
                    break;
                }

                currentPosition++; // Move to the next command
            }
        }

        private int HandleIfElseBlock(List<string> commands, int currentPosition)
        {
            var ifBlockCommands = new List<string>();
            var elseBlockCommands = new List<string>();
            var isInElseBlock = false;
            currentPosition++; // Move past the if command

            // Collect commands within if and else blocks
            while (currentPosition < commands.Count)
            {
                var com = commands[currentPosition];

                if (com.Equals("{", StringComparison.Ordinal))
                {
                    currentPosition++;
                    continue;
                }

                if (com.Equals("}", StringComparison.Ordinal))
                {
                    currentPosition++;
                    break;
                }

                if (com.Equals("else", StringComparison.OrdinalIgnoreCase))
                {
                    isInElseBlock = true;
                    currentPosition++;
                    continue;
                }

                if (isInElseBlock)
                {
                    elseBlockCommands.Add(com);
                }
                else
                {
                    ifBlockCommands.Add(com);
                }

                currentPosition++;
            }

            // Evaluate the if condition
            var userResponse = false; //GetUserResponse(); // This method should get user input or some condition

            if (userResponse)
            {
                //ExecuteCommands(ifBlockCommands);
            }
            else if (elseBlockCommands.Count > 0)
            {
                //ExecuteCommands(elseBlockCommands);
            }

            return currentPosition;
        }

        /// <summary>
        ///     Checks if the input is a jump command and extracts the jump position.
        /// </summary>
        /// <param name="input">The input command.</param>
        /// <param name="key">The key indicating the command type.</param>
        /// <param name="position">The extracted jump position.</param>
        /// <param name="commands"></param>
        /// <returns><c>true</c> if it is a jump command; otherwise, <c>false</c>.</returns>
        private static bool IsJumpCommand(string input, int key, out int position, IReadOnlyList<string> commands)
        {
            position = 0;

            var (status, label) = Irt.GetParameters(input, key, IrtConst.InternContainerCommands);

            if (status != IrtConst.ParameterCommand || string.IsNullOrEmpty(label)) return false;

            // Example logic to determine the jump position from the label
            position = FindLabelPosition(label, commands);

            return position >= 0;
        }

        /// <summary>
        ///     Finds the position of the label in the list of commands.
        /// </summary>
        /// <param name="label">The label to find.</param>
        /// <param name="commands">The commands.</param>
        /// <returns>
        ///     The position of the label, or -1 if not found.
        /// </returns>
        private static int FindLabelPosition(string label, IReadOnlyList<string> commands)
        {
            for (var i = 0; i < commands.Count; i++)
            {
                var input = commands[i];
                var check = Irt.CheckFormat(input, IrtConst.InternalLabel,label);
                if (check) // Customize this condition to match your label logic
                    return i;
            }

            return -1; // Label not found
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                // Dispose managed resources
                _irtHandleInternal = null;
            _prompt = null;

            _disposed = true;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="IrtHandleContainer" /> class.
        /// </summary>
        ~IrtHandleContainer()
        {
            Dispose(false);
        }
    }
}