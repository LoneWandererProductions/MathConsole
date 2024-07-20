﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Interpreter
 * FILE:        Interpreter/IrtIfElseBlock.cs
 * PURPOSE:     Contains the If else logik
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable ArrangeBraces_foreach

namespace Interpreter
{
    public sealed class IrtIfElseBlock
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