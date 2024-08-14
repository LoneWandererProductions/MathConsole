using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InterpreteTests
{
	internal class IfElseParserNew
	{
		private enum Symbol
		{
			None,
			LPar,
			RPar,
			Equals,
			Text,
			If,
			ElseIf,
			Else,
			OpenBrace,
			CloseBrace,
			Identifier
		}

		private List<string> _input; // Raw SQL with preprocessor directives.
		private int _currentLineIndex = 0;

		// Simulates variables used in conditions
		private readonly Dictionary<string, string> _variableValues = new()
		{
			{ "VariableA", "Case1" },
			{ "VariableB", "CaseX" }
		};
		private Symbol _sy; // Current symbol.
		private string _string; // Identifier or text line;
		private readonly Queue<string> _textQueue = new(); // Buffered text parts of a single line.
		private int _lineNo; // Current line number for error messages.
		private string _line; // Current line for error messages.

		/// <summary>
		/// Get the next line from the input.
		/// </summary>
		/// <returns>Input line or null if no more lines are available.</returns>
		private string GetLine()
		{
			if (_currentLineIndex >= _input.Count)
			{
				return null;
			}
			_line = _input[_currentLineIndex++];
			_lineNo = _currentLineIndex;
			return _line;
		}

		/// <summary>
		/// Get the next symbol from the input stream and store it in _sy.
		/// </summary>
		private void GetSy()
		{
			string s;
			if (_textQueue.Count > 0) // Buffered text parts available, use one from these.
			{
				s = _textQueue.Dequeue();
				switch (s.ToLower())
				{
					case "{":
						_sy = Symbol.OpenBrace;
						break;
					case "}":
						_sy = Symbol.CloseBrace;
						break;
					case "(":
						_sy = Symbol.LPar;
						break;
					case ")":
						_sy = Symbol.RPar;
						break;
					case "=":
						_sy = Symbol.Equals;
						break;
					case "if":
						_sy = Symbol.If;
						break;
					case "elseif":
						_sy = Symbol.ElseIf;
						break;
					case "else":
						_sy = Symbol.Else;
						break;
					default:
						_sy = Symbol.Identifier;
						_string = s;
						break;
				}
				return;
			}

			// Get next line from input.
			s = GetLine();
			if (s == null)
			{
				_sy = Symbol.None;
				return;
			}

			s = s.Trim(' ', '\t');
			if (s.Contains("if") || s.Contains("elseif") || s.Contains("else"))
			{
				// Split the line to get the directive keyword and possible conditions.
				string[] parts = Regex.Split(s, @"\s+");
				// The first part is the directive keyword (if, elseif, else)
				switch (parts[0].ToLower())
				{
					case "if":
						_sy = Symbol.If;
						break;
					case "elseif":
						_sy = Symbol.ElseIf;
						break;
					case "else":
						_sy = Symbol.Else;
						break;
					default:
						Error("Invalid directive '{0}'", parts[0]);
						break;
				}

				// Store the remaining parts for later.
				for (int i = 1; i < parts.Length; i++)
				{
					string part = parts[i].Trim(' ', '\t');
					if (part != "")
					{
						_textQueue.Enqueue(part);
					}
				}
			}
			else // We have an ordinary SQL text line.
			{
				_sy = Symbol.Text;
				_string = s;
			}
		}

		private void Error(string message, params object[] args)
		{
			// Make sure parsing stops here
			_sy = Symbol.None;
			_textQueue.Clear();
			_input.Clear();

			message = String.Format(message, args) + $" in line {_lineNo}\r\n\r\n{_line}";
			Output("------");
			Output(message);
			Console.WriteLine("!!! Error: " + message);
		}

		/// <summary>
		/// Writes the processed line to a (simulated) output stream.
		/// </summary>
		/// <param name="line">Line to be written to output</param>
		private static void Output(string line)
		{
			Console.WriteLine(line);
		}

		/// <summary>
		/// Starts the parsing process.
		/// </summary>
		public void Parse()
		{
			// Simulate an input stream.
			_input = new List<string>
			{
				"select column1",
				"from",
				"if (VariableA = Case1) {",
				"    if (VariableB = Case3) {",
				"        table3",
				"    } else {",
				"        table4",
				"    }",
				"else if (VariableA = Case2) {",
				"    table2",
				"} else {",
				"    defaultTable",
				"}"
			};

			// Clear previous parsing
			_textQueue.Clear();
			_currentLineIndex = 0;

			// Get first symbol and start parsing
			GetSy();
			if (LineSequence(true)) // Finished parsing successfully.
			{
				// TODO: Do something with the generated SQL
			}
			else // Error encountered.
			{
				Output("*** ABORTED ***");
			}
		}

		// The following methods parse according to the EBNF syntax.

		private bool LineSequence(bool writeOutput)
		{
			// EBNF:  LineSequence = { TextLine | IfStatement }.
			while (_sy is Symbol.Text or Symbol.If)
			{
				if (_sy == Symbol.Text)
				{
					if (!TextLine(writeOutput))
					{
						return false;
					}
				}
				else // _sy == Symbol.If
				{
					if (!IfStatement(writeOutput))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool TextLine(bool writeOutput)
		{
			// EBNF:  TextLine = <string>.
			if (writeOutput)
			{
				Output(_string);
			}
			GetSy();
			return true;
		}

		private bool IfStatement(bool writeOutput)
		{
			// EBNF:  IfStatement = IfLine Block { ElseIfLine Block } [ ElseLine Block ].
			if (IfLine(out bool result) && Block(writeOutput && result))
			{
				writeOutput &= !result; // Only one section can produce an output.
				while (_sy == Symbol.ElseIf)
				{
					GetSy();
					if (!ElseIfLine(out result))
					{
						return false;
					}
					if (!Block(writeOutput && result))
					{
						return false;
					}
					writeOutput &= !result; // Only one section can produce an output.
				}
				if (_sy == Symbol.Else)
				{
					GetSy();
					if (!Block(writeOutput))
					{
						return false;
					}
				}
				if (_sy != Symbol.CloseBrace)
				{
					Error("'}' expected to close the block");
					return false;
				}
				GetSy();
				return true;
			}
			return false;
		}

		private bool Block(bool writeOutput)
		{
			// EBNF:  Block = "{" LineSequence "}".
			if (_sy != Symbol.OpenBrace)
			{
				Error("'{' expected");
				return false;
			}
			GetSy();
			if (!LineSequence(writeOutput))
			{
				return false;
			}
			if (_sy != Symbol.CloseBrace)
			{
				Error("'}' expected");
				return false;
			}
			GetSy();
			return true;
		}

		private bool IfLine(out bool result)
		{
			// EBNF:  IfLine = "if" "(" Condition ")".
			result = false;
			GetSy();
			if (_sy != Symbol.LPar)
			{
				Error("'(' expected");
				return false;
			}
			GetSy();
			if (!Condition(out result))
			{
				return false;
			}
			if (_sy != Symbol.RPar)
			{
				Error("')' expected");
				return false;
			}
			GetSy();
			return true;
		}

		private bool Condition(out bool result)
		{
			// EBNF:  Condition = Identifier "=" Identifier.
			string variable;
			string expectedValue;

			result = false;
			// Identifier "=" Identifier
			if (_sy != Symbol.Identifier)
			{
				Error("Identifier expected");
				return false;
			}
			variable = _string; // The first identifier is a variable.
			GetSy();
			if (_sy != Symbol.Equals)
			{
				Error("'=' expected");
				return false;
			}
			GetSy();
			if (_sy != Symbol.Identifier)
			{
				Error("Value expected");
				return false;
			}
			expectedValue = _string;  // The second identifier is a value.

			// Search the variable
			if (_variableValues.TryGetValue(variable, out string variableValue))
			{
				result = variableValue == expectedValue; // Perform the comparison.
			}
			else
			{
				Error("Variable '{0}' not found", variable);
				return false;
			}

			GetSy();
			return true;
		}

		private bool ElseIfLine(out bool result)
		{
			// EBNF:  ElseIfLine = "elseif" "(" Condition ")".
			result = false;
			GetSy(); // "elseif" already processed here, we are only called if the symbol is "elseif"
			if (_sy != Symbol.LPar)
			{
				Error("'(' expected");
				return false;
			}
			GetSy();
			if (!Condition(out result))
			{
				return false;
			}
			if (_sy != Symbol.RPar)
			{
				Error("')' expected");
				return false;
			}
			GetSy();
			return true;
		}
	}
}
