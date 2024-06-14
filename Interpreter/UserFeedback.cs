using System.Collections.Generic;

namespace Interpreter
{
    public class UserFeedback
    {
        /// <summary>
        ///     Gets or sets the identifier. -1 if it is unused.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public int Id { get; set; }

        public bool Before { get; set; }

        public bool Introduction { get; set; }

        public Dictionary<AvailableFeedback, string> Options { get; set; }
    }
}