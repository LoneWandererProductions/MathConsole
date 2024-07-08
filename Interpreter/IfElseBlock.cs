/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IfElseBlock.cs
 * PURPOSE:     Contains the If else logik
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable ArrangeBraces_foreach

using System.Collections.Generic;

namespace Interpreter
{
    internal class IfElseBlock
    {
		internal string IfBlockCommands = string.Empty;

		internal string ElseBlockCommands = string.Empty;

		internal bool IsInElseBlock;

		internal int CurrentPosition;
	}
}