/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtHandleContainer.cs
 * PURPOSE:     Handles all the stuff with container
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Local

using System;

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
        ///     The irt handle internal
        /// </summary>
        private IrtHandleInternal _irtHandleInternal;

        /// <summary>
        /// The prompt
        /// </summary>
        private Prompt _prompt;

        /// <summary>
        ///     The disposed
        /// </summary>
        private bool _disposed;

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
        /// <param name="prompt"></param>
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
        /// <param name="inputString">Full input string.</param>
        internal void CommandContainer(string parameterPart, string inputString)
        {
            parameterPart = Irt.RemoveLastOccurrence(parameterPart, IrtConst.AdvancedClose);
            parameterPart = Irt.RemoveFirstOccurrence(parameterPart, IrtConst.AdvancedOpen);

            GenerateCommands(parameterPart);
        }

        /// <summary>
        /// Commands the batch execute.
        /// </summary>
        /// <param name="parameterPart">The parameter part.</param>
        /// <param name="inputString">The input string.</param>
        internal void CommandBatchExecute(string parameterPart, string inputString)
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
        /// </summary>
        /// <param name="parameterPart">The parameter part.</param>
        private void GenerateCommands(string parameterPart)
        {
            foreach (var com in Irt.SplitParameter(parameterPart, IrtConst.NewCommand))
            {
                //just because we run a container or a batch, we still have to log it
                _prompt.AddToLog(com);
                //mostly use the prompt to add a layer of security
                _prompt.ConsoleInput(com);
            }
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
            if (_disposed) return;
            if (!disposing) return;

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