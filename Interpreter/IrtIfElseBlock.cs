/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtIfElseBlock.cs
 * PURPOSE:     Contains the If else logic
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable ArrangeBraces_foreach

namespace Interpreter
{
    internal sealed class IrtIfElseBlock
    {
        internal string Condition { get; init; }
        internal string IfClause { get; init; }
        internal string ElseClause { get; init; }

        public override string ToString()
        {
            return $"If({Condition}) {{ {IfClause} }} Else {{ {ElseClause} }}";
        }
    }
}