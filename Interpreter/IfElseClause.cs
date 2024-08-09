namespace Interpreter
{
    public class IfElseClause
    {
        public string Parent { get; set; }
        public string IfClause { get; set; }
        public string ElseClause { get; set; }
        public int Layer { get; set; } // Depth level of the `if-else` structure
    }
}
