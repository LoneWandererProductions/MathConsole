namespace Interpreter
{
    internal class IrtRegister
    {
        /// <summary>
        /// The await input check, if true await correct answer
        /// </summary>
        /// <value>
        ///   <c>true</c> if [await input]; otherwise, <c>false</c>.
        /// </value>
        internal bool AwaitInput { get; set; }

        /// <summary>
        /// Gets or sets the awaited input Id
        /// </summary>
        /// <value>
        /// The awaited input Id.
        /// </value>
        internal int AwaitedInput { get; set; }

        /// <summary>
        /// Gets or sets the awaited output.
        /// </summary>
        /// <value>
        /// The awaited output.
        /// </value>
        internal OutCommand AwaitedOutput { get; set; }
    }
}