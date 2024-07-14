/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IfElseBlock.cs
 * PURPOSE:     Contains the If else logik
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable ArrangeBraces_foreach

namespace Interpreter
{
    public class IfElseBlock
    {
        public string Condition { get; set; }
        public string IfClause { get; set; }
        public string ElseClause { get; set; }

        public override string ToString()
        {
            return $"If({Condition}) {{ {IfClause} }} Else {{ {ElseClause} }}";
        }
    }
}