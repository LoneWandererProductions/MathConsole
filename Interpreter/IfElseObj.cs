using System.Collections.Generic;

namespace Interpreter
{
    public class IfElseObj
    {
        public int Id { get; set; }
        public int ParentId { get; set; }

        public int Position
        {
            get; set;
        }

        public int Layer { get; set; }
        public bool Else { get; set; }

        public bool Nested { get; set; }

        public Dictionary<int, (string, string)> Commands { get; set; }
        public string Input { get; set; }
    }
}