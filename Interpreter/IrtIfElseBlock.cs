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
    public sealed class IrtIfElseBlock
    {
        public string Condition { get; init; }
        public string IfClause { get; init; }
        public string ElseClause { get; init; }

        public override string ToString()
        {
            return $"If({Condition}) {{ {IfClause} }} Else {{ {ElseClause} }}";
        }
    }
}