﻿/*
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
        private const string InternalContainer = "CONTAINER";

        /// <summary>
        ///     The internal command batch execute (const). Value: "BATCHEXECUTE".
        /// </summary>
        private const string InternalBatchExecute = "BATCHEXECUTE";

        /// <summary>
        ///     The internal command print (const). Value: "PRINT".
        /// </summary>
        private const string InternalPrint = "PRINT";

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
        ///     The internal command if statement (const). Value: "IF".
        /// </summary>
        private const string InternalIf = "IF";

        /// <summary>
        ///     The internal command else, followed after if (const). Value: "ELSE".
        /// </summary>
        internal const string InternalElse = "ELSE";

        /// <summary>
        ///     The internal command Goto (const). Value: "GOTO".
        /// </summary>
        private const string InternalGoto = "GOTO";

        /// <summary>
        ///     The internal command label, used by goto (const). Value: "LABEL".
        /// </summary>
        internal const string InternalLabel = "LABEL";

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
        ///     The error feedback (const). Value: "No valid Options available.".
        /// </summary>
        internal const string ErrorFeedbackOptions = "No valid Options available.";

        /// <summary>
        ///     The error feedback Option not allowed (const). Value: "Option not allowed.".
        /// </summary>
        internal const string ErrorFeedbackOptionNotAllowed = "Option not allowed.";

        /// <summary>
        ///     The error Internal Extension not found (const). Value: "Unknown Internal extension command.".
        /// </summary>
        internal const string ErrorInternalExtensionNotFound = "Unknown Internal extension command.";

        /// <summary>
        ///     The feedback Message (const). Value: "You selected: ".
        /// </summary>
        internal const string FeedbackMessage = "You selected: ";

        /// <summary>
        ///     The parenthesis error (const). Value: "Wrong parenthesis".
        /// </summary>
        internal const string ParenthesisError = "Wrong parenthesis";

        /// <summary>
        ///     The key word not found error (const). Value: "error KeyWord not Found: ".
        /// </summary>
        internal const string KeyWordNotFoundError = "error KeyWord not Found: ";

        /// <summary>
        ///     The key word not found error (const). Value: "error KeyWord not Found: ".
        /// </summary>
        internal const string JumpLabelNotFoundError = "error jump label not found: ";

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
        ///     Help with Parameter Id. (const). Value: "1".
        /// </summary>
        internal const int InternalHelpWithParameter = 1;

        /// <summary>
        ///     Important Command Id for Container. (const). Value: "8".
        /// </summary>
        internal const int InternalContainerId = 8;

        /// <summary>
        ///     Important Command Id for batch files. (const). Value: "9".
        /// </summary>
        internal const int InternalBatchId = 9;

        /// <summary>
        ///     The internal check, if Parameter is empty, since the brackets are expected. (const). Value: "()".
        /// </summary>
        internal static readonly string InternalEmptyParameter = string.Concat(BaseOpen, BaseClose);

        /// <summary>
        ///     The Help feedback Object, only used for the Internal Help Extension
        /// </summary>
        private static readonly UserFeedback HelpFeedback = new()
        {
            Before = true,
            Message = "You now have the following Options:",
            Options = new Dictionary<AvailableFeedback, string>
            {
                {
                    AvailableFeedback.Yes,
                    "If you want to execute the Command type yes"
                },
                {
                    AvailableFeedback.No,
                    " if you want to stop executing the Command."
                }
            }
        };

        /// <summary>
        ///     The internal feedback
        /// </summary>
        internal static readonly Dictionary<int, UserFeedback> InternalFeedback = new()
        {
            {
                -1,
                HelpFeedback
            }
        };

        /// <summary>
        ///     The internal Extension commands, will be used for all external UserSpaces.
        /// </summary>
        internal static readonly Dictionary<int, InCommand> InternalExtensionCommands = new()
        {
            {
                0,
                new InCommand
                {
                    Command = InternalExtensionUse,
                    Description = "use(parameter) : use the provided parameter as Userspace, if it exists.",
                    ParameterCount = 1
                }
            },
            {
                1,
                new InCommand
                {
                    Command = InternalHelpExtension,
                    Description =
                        "help(parameter) : displays the help for the command and asks if you want to execute it, not compatible with internal commands.",
                    ParameterCount = 0
                }
            }
        };

        /// <summary>
        ///     The Dictionary for internal commands, all implemented and tested
        /// </summary>
        internal static readonly Dictionary<int, InCommand> InternCommands = new()
        {
            {
                0,
                new InCommand
                {
                    Command = InternalCommandHelp,
                    Description = "Help : List all external Commands.",
                    ParameterCount = 0
                }
            },
            {
                InternalHelpWithParameter,
                new InCommand
                {
                    Command = InternalCommandHelp,
                    Description =
                        "Help : Help with Parameter is an overload of help, provides Information about specified command.",
                    ParameterCount = 1
                }
            },
            {
                2,
                new InCommand
                {
                    Command = InternalCommandList,
                    Description = "List: List all external Commands.",
                    ParameterCount = 0
                }
            },
            {
                3,
                new InCommand
                {
                    Command = InternalUsing,
                    Description = "Using : Displays current Commands available and the one currently in use.",
                    ParameterCount = 0
                }
            },
            {
                4,
                new InCommand
                {
                    Command = InternalUse,
                    Description = "Use : Type use(namespace) to switch to command namespace",
                    ParameterCount = 1
                }
            },
            {
                5,
                new InCommand
                {
                    Command = InternalLogInfo,
                    Description = "Loginfo : statistics about the current log",
                    ParameterCount = 0
                }
            },
            {
                6,
                new InCommand
                {
                    Command = InternalErrorLog,
                    Description = "Log : Enumerate all Error Log entries.",
                    ParameterCount = 0
                }
            },
            {
                7,
                new InCommand
                {
                    Command = InternalLogFull,
                    Description = "Logfull : Enumerate full Log",
                    ParameterCount = 0
                }
            },
            {
                InternalContainerId,
                new InCommand
                {
                    Command = InternalContainer,
                    Description =
                        "Container : Holds a set of commands and executes them sequential, use Container{Command1; Command2; .... } and ; is the Separator that states that a new command follows.",
                    ParameterCount = 0
                }
            },
            {
                InternalBatchId,
                new InCommand
                {
                    Command = InternalBatchExecute,
                    Description = "Batchexecute : Loads a file and executes the commands in it, similar to Container.",
                    ParameterCount = 1
                }
            },
            {
                10,
                new InCommand
                {
                    Command = InternalPrint,
                    Description = "Print : Just print the content within in the parenthesis.",
                    ParameterCount = 1
                }
            }
        };

        /// <summary>
        ///     The intern container commands
        /// </summary>
        internal static readonly Dictionary<int, InCommand> InternContainerCommands = new()
        {
            {
                0,
                new InCommand
                {
                    Command = InternalIf,
                    Description =
                        "If : intended for batch commands and Container, aks for condition and executes everything after the following {}. The Parameter is the ",
                    ParameterCount = 1
                }
            },
            {
                1,
                new InCommand
                {
                    Command = InternalElse,
                    Description =
                        "Else :must be followed after an If and the {} if not we will throw an error, if the If clause was wrong everything after else {} will be executed.",
                    ParameterCount = 0
                }
            },
            {
                2,
                new InCommand
                {
                    Command = InternalGoto,
                    Description =
                        "Goto : intended for batch commands and Container, if reached, code will jump to Label with Parameter equal to Goto Parameter, else we will get an error.",
                    ParameterCount = 1
                }
            },
            {
                3,
                new InCommand
                {
                    Command = InternalLabel,
                    Description = "label : intended for batch commands and Container, Entry point for Goto Command.",
                    ParameterCount = 1
                }
            }
        };


        /// <summary>
        ///     Basic internal Help
        /// </summary>
        /// <summary>
        ///     Basic internal Help
        /// </summary>
        internal static string HelpGeneric => HelpInfo();

        /// <summary>
        ///     Helps the information.
        /// </summary>
        /// <returns>Collected Infos about the commands.</returns>
        private static string HelpInfo()
        {
            return string.Join(
                Environment.NewLine,
                "Basic prompt, Version : 0.3. Author: Peter Geinitz (Wayfarer), not context sensitive",
                "Type Help or Help(Keyword) for specific help",
                "Basic Syntax: Verb(Parameter, ...)",
                "System Commands:",
                $"{InternCommands[0].Command} : {InternCommands[0].Description}",
                $"{InternCommands[1].Command} : {InternCommands[1].Description}",
                $"{InternCommands[2].Command} : {InternCommands[2].Description}",
                $"{InternCommands[3].Command} : {InternCommands[3].Description}",
                $"{InternCommands[4].Command} : {InternCommands[4].Description}",
                $"{InternCommands[5].Command} : {InternCommands[5].Description}",
                $"{InternCommands[6].Command} : {InternCommands[6].Description}",
                $"{InternCommands[7].Command} : {InternCommands[7].Description}",
                $"{InternCommands[InternalContainerId].Command} : {InternCommands[InternalContainerId].Description}",
                $"{InternCommands[InternalBatchId].Command} : {InternCommands[InternalBatchId].Description}",
                $"{InternCommands[10].Command} : {InternCommands[10].Description}",
                "As a word of warning, do not try to overwrite internal Commands, they will be always executed first. Duplicate commands in different Namespaces are allowed otherwise.",
                "Furthermore, there are Extension Commands that alter the behaviour of all Commands. They are added with a '.' to an existing command.",
                "The Internal Extensions are:",
                $"{InternalExtensionCommands[0].Command} : {InternalExtensionCommands[0].Description}",
                $"{InternalExtensionCommands[1].Command} : {InternalExtensionCommands[1].Description}",
                "The following are internal Commands used in Container Constructs and Batch Commands:",
                $"{InternContainerCommands[0].Command} : {InternContainerCommands[0].Description}",
                $"{InternContainerCommands[1].Command} : {InternContainerCommands[1].Description}",
                $"{InternContainerCommands[2].Command} : {InternContainerCommands[2].Description}",
                $"{InternContainerCommands[3].Command} : {InternContainerCommands[3].Description}"
            );
        }
    }
}