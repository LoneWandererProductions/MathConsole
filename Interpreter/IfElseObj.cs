using ExtendedSystemObjects;

namespace Interpreter
{
    internal class IfElseObj
    {
        internal int Id { get; init; }
        internal int ParentId { get; init; }

        internal int Position { get; init; }

        internal int Layer { get; init; }
        internal bool Else { get; init; }

        internal bool Nested { get; set; }

        internal CategorizedDictionary<int, string> Commands { get; set; }

        internal string Input { get; init; }
    }
}