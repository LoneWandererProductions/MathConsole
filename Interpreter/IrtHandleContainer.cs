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
        private readonly IrtHandleInternal _irtHandleInternal;

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
        internal IrtHandleContainer(IrtHandleInternal irtHandleInternal)
        {
            _irtHandleInternal = irtHandleInternal;
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

            _irtHandleInternal.GenerateCommands(parameterPart);
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
            if (!_disposed)
            {
                if (disposing)
                    // Dispose managed resources
                    _irtHandleInternal?.Dispose();

                // Set large fields to null
                // Example: _largeManagedResource = null;

                _disposed = true;
            }
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