namespace Interpreter
{
    internal sealed class IrtFeedbackInputEventArgs
    {
        internal string Input { get; set; }
        internal string RequestId { get; set; }
        internal int BranchId { get; set; }
        internal int Key { get; set; }
        internal string Command { get; set; }
        internal OutCommand AwaitedOutput { get; set; }
        internal AvailableFeedback Answer { get; set; }
    }
}