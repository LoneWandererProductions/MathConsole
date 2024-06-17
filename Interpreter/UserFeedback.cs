using System.Collections.Generic;

namespace Interpreter
{
    public class UserFeedback
    {
        public bool Before { get; set; }

        public bool Introduction { get; set; }

        public Dictionary<AvailableFeedback, string> Options { get; set; }
    }
}