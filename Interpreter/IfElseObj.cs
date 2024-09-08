using ExtendedSystemObjects;

namespace Interpreter
{
    public class IfElseObj
    {
        public int Id { get; init; }
        public int ParentId { get; init; }

        public int Position { get; init; }

        public int Layer { get; init; }
        public bool Else { get; init; }

        public bool Nested { get; set; }

        public CategorizedDictionary<int, string> Commands { get; set; }

        public string Input { get; init; }
    }
}