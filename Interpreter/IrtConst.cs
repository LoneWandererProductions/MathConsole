/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtConst.cs
 * PURPOSE:     The Command Constants that are delivered with the dll
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    ///     IrtConst contains all strings class.
    /// </summary>
    internal static class IrtConst
    {
        /// <summary>
        ///     Regex Pattern, Find periods that are not inside {} or (), (const). Value: @"\.(?![^{}]*\})(?![^(]*\))".
        /// </summary>
        internal const string RegexParenthesisOutsidePattern = @"\.(?![^{}]*\})(?![^(]*\))";

        /// <summary>
        ///     Regex Pattern to remove whitespace before '(' and '{', (const). Value: @"\s*([(){}])\s*".
        /// </summary>
        internal const string RegexParenthesisWellFormedPatternLeft = @"\s+(?=[({])";

        /// <summary>
        ///     Regex  Pattern to remove whitespace after ')' and '}', (const). Value:  @"(?&lt;=[)}])\s+".
        /// </summary>
        internal const string RegexParenthesisWellFormedPatternRight = @"(?<=[)}])\s+";

        /// <summary>
        ///     Regex  Pattern to remove all whitespace, (const). Value:  @"\s+".
        /// </summary>
        internal const string RegexRemoveWhiteSpace = @"\s+";

        /// <summary>
        ///     Separator (const). Value: ", ".
        /// </summary>
        internal const string Separator = " , ";

        /// <summary>
        ///     Empty Parameter (const). Value:  "()".
        /// </summary>
        internal const string EmptyParameter = "()";

        /// <summary>
        ///     The internal command container (const). Value: "CONTAINER".
        /// </summary>
        private const string InternalCommandContainer = "CONTAINER";

        /// <summary>
        ///     The internal command batch execute (const). Value: "BATCHEXECUTE".
        /// </summary>
        private const string InternalCommandBatchExecute = "BATCHEXECUTE";

        /// <summary>
        ///     The internal command help (const). Value: "HELP".
        /// </summary>
        internal const string InternalCommandHelp = "HELP";

        /// <summary>
        ///     The internal command list (const). Value: "LIST".
        /// </summary>
        private const string InternalCommandList = "LIST";

        /// <summary>
        ///     The internal Namespace (const). Value: "INTERNAL".
        /// </summary>
        internal const string InternalNameSpace = "INTERNAL";

        /// <summary>
        ///     The internal command using (const). Value: "USING".
        /// </summary>
        private const string InternalUsing = "USING";

        /// <summary>
        ///     The internal extension command use (const). Value: "Use".
        /// </summary>
        private const string InternalExtensionUse = "USE";

        /// <summary>
        ///     The internal extension command help (const). Value: "HELP".
        /// </summary>
        private const string InternalHelpExtension = "HELP";

        /// <summary>
        ///     The internal command use (const). Value: "USE".
        /// </summary>
        private const string InternalUse = "USE";

        /// <summary>
        ///     The internal command Log (const). Value: "LOG".
        /// </summary>
        private const string InternalErrorLog = "LOG";

        /// <summary>
        ///     The internal command Log info (const). Value: "LOGINFO".
        /// </summary>
        private const string InternalLogInfo = "LOGINFO";

        /// <summary>
        ///     The internal command Log full (const). Value: "LOGFULL".
        /// </summary>
        private const string InternalLogFull = "LOGFULL";

        /// <summary>
        ///     The error no commands provided (const). Value: "No Commands were provided".
        /// </summary>
        internal const string ErrorNoCommandsProvided = "No Commands were provided";

        /// <summary>
        ///     The error UserSpace not Found (const). Value: "Error UserSpace not found".
        /// </summary>
        internal const string ErrorUserSpaceNotFound = "Error UserSpace not found";

        /// <summary>
        ///     The error not initialized (const). Value: "Error please initiate the Prompt first.".
        /// </summary>
        internal const string ErrorNotInitialized = "Error please initiate the Prompt first.";

        /// <summary>
        ///     The error File not found (const). Value: "Error please initiate the Prompt first.".
        /// </summary>
        internal const string ErrorFileNotFound = "Error please initiate the Prompt first.";

        /// <summary>
        ///     The error for Extensions (const). Value: "Extension provided produced Errors: ".
        /// </summary>
        internal const string ErrorExtensions = "Extension provided produced Errors: ";

        /// <summary>
        ///     The error for Invalid Input (const). Value: "Input was null or empty.".
        /// </summary>
        internal const string ErrorInvalidInput = "Input was null or empty.";

        /// <summary>
        ///     The parenthesis error (const). Value: "Wrong parenthesis".
        /// </summary>
        internal const string ParenthesisError = "Wrong parenthesis";

        /// <summary>
        ///     The key word not found error (const). Value: "error KeyWord not Found: ".
        /// </summary>
        internal const string KeyWordNotFoundError = "error KeyWord not Found: ";

        /// <summary>
        ///     The syntax error (const). Value: "Error in the Syntax".
        /// </summary>
        internal const string SyntaxError = "Error in the Syntax: ";

        /// <summary>
        ///     The message info (const). Value: "Information: ".
        /// </summary>
        internal const string MessageInfo = "Information: ";

        /// <summary>
        ///     The message error (const). Value: "Error: ".
        /// </summary>
        internal const string MessageError = "Error: ";

        /// <summary>
        ///     The message warning (const). Value: "Warning: ".
        /// </summary>
        internal const string MessageWarning = "Warning: ";

        /// <summary>
        ///     The message error count (const). Value: "Error Count: ".
        /// </summary>
        internal const string MessageErrorCount = "Error Count: ";

        /// <summary>
        ///     The message log Count (const). Value: "Log Count: ".
        /// </summary>
        internal const string MessageLogCount = "Log Count: ";

        /// <summary>
        ///     The message log statistics (const). Value: "General Information about the Log.".
        /// </summary>
        internal const string MessageLogStatistics = "General Information abount the Log.";

        /// <summary>
        ///     The end (const). Value: ";".
        /// </summary>
        internal const string End = ";";

        /// <summary>
        ///     The Active (const). Value: "Active Using: ".
        /// </summary>
        internal const string Active = "Active Using: ";

        /// <summary>
        ///     The information startup (const). Value: "Interpreter started up".
        /// </summary>
        internal const string InformationStartup = "Interpreter started up";

        /// <summary>
        ///     The information Namespace switch (const). Value: "Namespace switched to: ".
        /// </summary>
        internal const string InformationNamespaceSwitch = "Namespace switched to: ";

        /// <summary>
        ///     The format description (const). Value: " Description: ".
        /// </summary>
        internal const string FormatDescription = " Description: ";

        /// <summary>
        ///     The format count (const). Value: " Parameter Count: ".
        /// </summary>
        internal const string FormatCount = " Parameter Count: ";

        /// <summary>
        ///     The open Clause, Standard is'('
        /// </summary>
        internal const char BaseOpen = '(';

        /// <summary>
        ///     The advanced open Clause, Standard is'{'
        /// </summary>
        internal const char AdvancedOpen = '{';

        /// <summary>
        ///     The close Clause, Standard is')'
        /// </summary>
        internal const char BaseClose = ')';

        /// <summary>
        ///     The advanced close Clause, Standard is'}'
        /// </summary>
        internal const char AdvancedClose = '}';

        /// <summary>
        ///     The Splitter Clause, Standard is','
        /// </summary>
        internal const char Splitter = ',';

        /// <summary>
        ///     The Splitter for a new Command, Standard is';'
        /// </summary>
        internal const char NewCommand = ';';

        /// <summary>
        ///     Indicator for comment, mostly used for batch files, (const). Value: "--".
        /// </summary>
        internal const string CommentCommand = "--";

        /// <summary>
        ///     The error. (const). Value: "-1".
        /// </summary>
        internal const int Error = -1;

        /// <summary>
        ///     The no split occurred. (const). Value: "2".
        /// </summary>
        internal const int NoSplitOccurred = 0;

        /// <summary>
        ///     The extension has a Parameter mismatch. (const). Value: "1".
        /// </summary>
        internal const int ParameterMismatch = 1;

        /// <summary>
        ///     The extension has a Parameter mismatch. (const). Value: "2".
        /// </summary>
        internal const int ParenthesisMismatch = 2;

        /// <summary>
        ///     The Internal extension found. (const). Value: "3".
        /// </summary>
        internal const int ExtensionFound = 3;

        /// <summary>
        ///     If Command  is Batch expression. (const). Value: "0".
        /// </summary>
        internal const int BatchCommand = 0;

        /// <summary>
        ///     If Command has Parameter. (const). Value: "1".
        /// </summary>
        internal const int ParameterCommand = 1;

        /// <summary>
        ///     The internal check, if Parameter is empty, since the brackets are expected. (const). Value: "()".
        /// </summary>
        internal static readonly string InternalEmptyParameter = string.Concat(BaseOpen, BaseClose);

        /// <summary>
        ///     The internal Extension commands
        /// </summary>
        internal static readonly Dictionary<int, InCommand> InternalExtensionCommands = new()
        {
            {
                0,
                new InCommand
                {
                    Command = InternalHelpExtension,
                    Description =
                        "help(parameter) : displays the help for the command and asks if you want to execute it.",
                    ParameterCount = 0
                }
            },
            {
                1,
                new InCommand
                {
                    Command = InternalHelpExtension,
                    Description =
                        "help(parameter) : displays the help for the command and asks if you want to execute it.",
                    ParameterCount = 0
                }
            }
        };

        /// <summary>
        ///     Basic internal Help
        /// </summary>
        internal static readonly string HelpGeneric = string.Join(
            Environment.NewLine,
            "Basic prompt, Version : 0.3. Author: Peter Geinitz (Wayfarer), not context sensitive",
            "Type Help or Help(Keyword) for a specific help",
            "Basic Syntax: Verb (Parameter, ...)",
            "Basic Syntax: Verb (Parameter, ...)",
            "System Commands:",
            $"{InternalCommandHelp} : Basic help and specific help for provided Commands",
            $"{InternalCommandList} : List all external Commands",
            $"{InternalUsing} : Current Commands available and the one currently in use",
            $"{InternalUse} : Type use(namespace) to switch to command namespace",
            $"{InternalLogInfo} : Statistics about the current log",
            $"{InternalErrorLog} : Enumerate all Error Log entries",
            $"{InternalLogFull} : Enumerate full Log",
            $"{InternalCommandContainer} : Holds a set of commands and executes them sequentially, use Container{{Command1; Command2; .... }}; ';' is the separator that indicates a new command follows."
        );


        /// <summary>
        ///     The Dictionary for internal commands
        /// </summary>
        internal static readonly Dictionary<int, InCommand> InternCommands = new()
        {
            {
                0,
                new InCommand
                {
                    Command = InternalCommandHelp,
                    Description = "Help : List all external Commands",
                    ParameterCount = 0
                }
            },
            {
                1,
                new InCommand
                {
                    Command = InternalCommandList,
                    Description = "List: Calculate the statistic data for the loaded file.",
                    ParameterCount = 0
                }
            },
            {
                2,
                new InCommand
                {
                    Command = InternalUsing,
                    Description = "Using : Current Commands available and the one currently in use.",
                    ParameterCount = 0
                }
            },
            {
                3,
                new InCommand
                {
                    Command = InternalUse,
                    Description = "Use : Type use(namespace) to switch to command namespace",
                    ParameterCount = 1
                }
            },
            {
                4,
                new InCommand
                {
                    Command = InternalLogInfo,
                    Description = "Loginfo : statistics about the current log",
                    ParameterCount = 0
                }
            },
            {
                5,
                new InCommand
                {
                    Command = InternalErrorLog,
                    Description = "Log : Enumerate all Error Log entries.",
                    ParameterCount = 0
                }
            },
            {
                6,
                new InCommand
                {
                    Command = InternalLogFull,
                    Description = "Logfull : Enumerate full Log",
                    ParameterCount = 0
                }
            },
            {
                7,
                new InCommand
                {
                    Command = InternalCommandContainer,
                    Description =
                        "Container : Holds a set of commands and executes them sequential, use Container{Command1; Command2; .... } and ; is the Separator that states that a new command follows.",
                    ParameterCount = 0
                }
            },
            {
                8,
                new InCommand
                {
                    Command = InternalCommandBatchExecute,
                    Description = "Batchexecute : Loads a file and executes the commands in it, similar to Container.",
                    ParameterCount = 1
                }
            }
        };
    }
}