/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtHandleExtensionInternal.cs
 * PURPOSE:     Handle the Input for Internal Extensions
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Local

using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <inheritdoc />
    /// <summary>
    ///     Helper class for handling internal extensions.
    /// </summary>
    internal sealed class IrtHandleExtensionInternal : IDisposable
    {
        /// <summary>
        ///     Dictionary of available commands.
        /// </summary>
        private Dictionary<int, InCommand> _commands;

        /// <summary>
        ///     Indicates whether the object has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Instance of IrtHandleInternal for handling internal command logic.
        /// </summary>
        private IrtHandleInternal _irtHandleInternal;

        /// <summary>
        ///     Instance of IrtHandlePrompt for internal use.
        /// </summary>
        private IrtParser _irtHandlePrompt;

        /// <summary>
        ///     Instance of Prompt to handle command input/output.
        /// </summary>
        private Prompt _prompt;

        /// <summary>
        ///     Prevents a default instance of the <see cref="IrtHandleExtensionInternal" /> class from being created.
        /// </summary>
        private IrtHandleExtensionInternal()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IrtHandleExtensionInternal" /> class with specified parameters.
        /// </summary>
        /// <param name="irtPrompt">Instance of IrtHandlePrompt.</param>
        /// <param name="commands">Dictionary of commands.</param>
        /// <param name="prompt">Instance of Prompt.</param>
        /// <param name="irtInternal">Instance of IrtHandleInternal.</param>
        public IrtHandleExtensionInternal(IrtParser irtPrompt, Dictionary<int, InCommand> commands, Prompt prompt,
            IrtHandleInternal irtInternal)
        {
            _irtHandlePrompt = irtPrompt;
            _commands = commands;
            _prompt = prompt;
            _irtHandleInternal = irtInternal;
            _disposed = false;
        }

        /// <summary>
        ///     Releases all resources used by the <see cref="IrtHandleExtensionInternal" /> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Processes the internal extension based on the given extension command.
        /// </summary>
        /// <param name="extension">The extension command to process.</param>
        internal void ProcessExtensionInternal(ExtensionCommands extension)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(IrtHandleExtensionInternal));

            switch (extension.ExtensionCommand)
            {
                case 0:
                    // Switch namespace
                    _prompt.SwitchNameSpaces(extension.ExtensionParameter[0]);
                    _prompt.ConsoleInput(extension.BaseCommand);
                    break;

                case 1:
                    // Display help and ask for user feedback
                    var key = Irt.CheckForKeyWord(extension.BaseCommand, IrtConst.InternCommands);
                    if (key != IrtConst.Error)
                    {
                        var command = IrtConst.InternCommands[key];

                        using (var irtInternal = new IrtHandleInternal(IrtConst.InternCommands,
                                   IrtConst.InternalNameSpace, _prompt))
                        {
                            irtInternal.ProcessInput(IrtConst.InternalHelpWithParameter, command.Command);
                        }

                        _prompt.CommandRegister = new IrtFeedback
                        {
                            AwaitedInput = -1,
                            AwaitInput = true,
                            IsInternalCommand = true,
                            InternalInput = extension.BaseCommand,
                            CommandHandler = _irtHandleInternal,
                            Key = key
                        };
                    }
                    else
                    {
                        var com = _irtHandlePrompt.ProcessInput(extension.BaseCommand);
                        var command = _commands[com.Command];
                        _irtHandleInternal.ProcessInput(IrtConst.InternalHelpWithParameter, command.Command);

                        _prompt.CommandRegister = new IrtFeedback
                        {
                            AwaitedInput = -1,
                            AwaitInput = true,
                            AwaitedOutput = com
                        };
                    }

                    break;

                default:
                    _prompt.SendLogs(nameof(ProcessExtensionInternal), IrtConst.ErrorInternalExtensionNotFound);
                    break;
            }
        }

        /// <summary>
        ///     Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     Indicates whether the method call comes from a Dispose method (true) or from a finalizer
        ///     (false).
        /// </param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources
                _commands = null;

                _irtHandlePrompt = null;
                _prompt = null;
                _irtHandleInternal = null;
            }

            // Dispose unmanaged resources here if needed

            _disposed = true;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="IrtHandleExtensionInternal" /> class.
        /// </summary>
        ~IrtHandleExtensionInternal()
        {
            Dispose(false);
        }
    }
}